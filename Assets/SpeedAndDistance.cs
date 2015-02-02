using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/**
 * Script usado para calcular la distancia recorrida y la velocidad de desplazamiento aproximadas de un
 * dispositivo movil con base en las coordenadas obtenidas mediante GPS.
 */
public class SpeedAndDistance : MonoBehaviour {
	public Text text;
	/**
	 * El desplazamiento registrado mediante el GPS en metros.
	 */
	[HideInInspector]
	public float accumulatedDistance = 0.0f;
	/**
	 * Tiempo acumulado del recorrido en segundos.
	 */
	[HideInInspector]
	public float runningTime = 0.0f;
	private string value = "";
	/**
	 * Indica si ya se inicializo la posicion inicial del dispositivo.
	 */
	private bool initialized = false;
	/**
	 * Intervalo de actualizacion de la localizacion del GPS.
	 */
	public float updateInterval = 5f;
	/**
	 * Indica si el dispositivo se debe bloquear durante la ejecucion
	 * de la aplicacion para ahorrar energia, en este caso la aplicacion no podria seguir en ejecucion
	 * durante el tiempo que el dispositivo se bloquee para ahorrar energia.
	 */
	public bool turnOff = false;
	/**
	 * Tiempo en segundos que se debe esperar para obtener la ubicacion del GPS.
	 */
	public int maxWait = 30;
	//distancia en metros requerida para actualizar la localizacion GPS
	public float updateDistance = 0.1f;
	//precision deseada de la medicion de la ubicacion GPS
	public float desiredAccuracy = 1f;
	private float currentTime;
	//radio de la Tierra en Metros
	private const float R = 6371f*1000f;
	//ultima localizacion 
	private LocationInfo lastLocation;
	// Use this for initialization
	void Start() {
		currentTime = Time.time;
		//si turnoff es false el dispositivo no se bloqueara automaticamente cuando este programado,
		//esto permite que la aplicacion siga corriendo en todo momento, de lo contrario una vez
		//el dispositivo se bloquee la aplicacion dejara de funcionar
		if (!turnOff) {
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}
	}
	
	/**
	 * Este metodo permite calcular la distancia aproximada entre dos puntos usando coordenadas de 
	 * latitud y longitud. Para calcular la distancia se uso la formula de Haversine encontrada en
	 * http://www.movable-type.co.uk/scripts/latlong.html. Se omitio la altitud dado que se considera que
	 * las mediciones se llevaran a cabo en distancias cortas, por lo tanto la altitud no resulta
	 * relevante en este caso.
	 */
	public float Distance(LocationInfo l1, LocationInfo l2)
	{
		float fi1 = Mathf.Deg2Rad * l1.latitude;
		float fi2 = Mathf.Deg2Rad * l2.latitude;
		float deltaFi = Mathf.Deg2Rad * (l2.latitude - l1.latitude);
		float deltaLambda = Mathf.Deg2Rad * (l2.longitude - l1.longitude);
		float a = Mathf.Sin (deltaFi / 2) * Mathf.Sin (deltaFi / 2) +
						Mathf.Cos (fi1) * Mathf.Cos (fi2) *
						Mathf.Sin (deltaLambda / 2) * Mathf.Sin (deltaLambda / 2);
		float c = 2 * Mathf.Atan2 (Mathf.Sqrt (a), Mathf.Sqrt (1 - a));
		return R*c;
	}
	
	/**
	 * Rutina llamada cada vez que se desee actualizar la localizacion del dispositivo mediante GPS.
	 */
	public IEnumerator getLocation()
	{
		if (!Input.location.isEnabledByUser) {
			text.text = "No se ha habilitado el GPS en el dispositivo";
			yield return null;
		}
		//inicializa el GPS con la precision y distancia de actualizacion especificadas
		Input.location.Start (desiredAccuracy, updateDistance);
		int wait = maxWait;
		//ciclo en el cual se espera un maximo de maxWait segundos para que el
		//dispositivo active el GPS y muestre la localizacion
		while (Input.location.status == LocationServiceStatus.Initializing && wait > 0) {
			yield return new WaitForSeconds(1);
			wait--;
		}
		//se espero mas del tiempo maximo definido
		if (wait < 1) {
			text.text = "Timed out";
			yield return null;
		}
		//Por alguna razon fallo la localizacion del dispositivo
		if (Input.location.status == LocationServiceStatus.Failed) {
			text.text = "Unable to determine device location";
			yield return null;
		} else {
			text.text = "Location: " + Input.location.lastData.latitude.ToString() + " " + Input.location.lastData.longitude.ToString() + " " + Input.location.lastData.altitude.ToString() + " " + Input.location.lastData.horizontalAccuracy + " " + runningTime+" secs ";
			//se acumula la distancia recorrida
			if(initialized)
			{
				accumulatedDistance += Distance (lastLocation, Input.location.lastData);
				text.text += " Distance = "+accumulatedDistance+" meters. Speed: "+getKmH(Speed())+" km/h";
			}
			lastLocation = Input.location.lastData;
			if(!initialized)
			{
				initialized = true;
				runningTime = 0.0f;
			}
		}
		//Input.compass.enabled = true;
		//ext.text += " Magnetometer reading: " + Input.compass.rawVector.ToString ();
		//text.text += " Gyro reading: " + gyro.rotationRate+" "+gyro.gravity+" "+gyro.attitude+" "+gyro.updateInterval;
		//text.text += " Acceleration reading: " + Input.acceleration;
		//text.text += " " + Input.touchCount;
		Input.location.Stop();
	}
	/**
	 * Rapidez media con la que se ha movido la persona durante su recorrido,
	 * para esto se toma en cuenta el tiempo acumulado y la distancia acumulada durante ese
	 * periodo de tiempo. Este metodo retorna la velocidad en metros/segundo
	 */
	public float Speed()
	{
		if (runningTime <= 0.0f) {
			return 0.0f;
		}
		return accumulatedDistance / runningTime;
	}
	/**
	 * Permite obtener la rapidez en kilimetros/hora apartir de la rapidez en metros/segundo especificada como
	 * argumento
	 */
	public float getKmH(float ms)
	{
		return ms * 3600 / 1000;
	}
	void FixedUpdate()
	{
		//se contabiliza el tiempo desde la ultima actualizacion
		runningTime += Time.deltaTime;
		if (Time.time - currentTime >= updateInterval) {
			//Tiempo de la ultima actualizacion
			currentTime = Time.time;
			//actualiza la ubicacion actual a partir del dispositivo
			//en una corutina que corre en un thread alternativo
			StartCoroutine(getLocation());

		}

	}

}
