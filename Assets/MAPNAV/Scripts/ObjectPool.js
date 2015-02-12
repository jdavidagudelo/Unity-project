	#pragma strict
	public static var instance:ObjectPool;
    /// <summary>
    /// The object prefabs which the pool can handle.
    /// </summary>
    public var objectPrefabs:GameObject[];

    

    /// <summary>

    /// The pooled objects currently available.

    /// </summary>

    public var pooledObjects:ArrayList[];

    

    /// <summary>

    /// The amount of objects of each type to buffer.

    /// </summary>

    public var amountToBuffer:int[];

    

    public var defaultBufferAmount:int = 3;

    

    /// <summary>

    /// The container object that we will keep unused pooled objects so we dont clog up the editor with objects.

    /// </summary>

    protected var  containerObject:GameObject;

    

    function Awake ()

    {

        instance = this;

    }

    

    // Use this for initialization

    function Start ()

    {
		initPool();
    }

    public function initPool()
	{
		 containerObject = new GameObject("ObjectPool");

        

        //Loop through the object prefabs and make a new list for each one.

        //We do this because the pool can only support prefabs set to it in the editor,

        //so we can assume the lists of pooled objects are in the same order as object prefabs in the array

        pooledObjects = new ArrayList[objectPrefabs.Length];

        

        var i:int = 0;

        for( var objectPrefab:GameObject  in objectPrefabs )

        {
			
			
            pooledObjects[i] = new ArrayList();  

            

            var bufferAmount:int;

            

            if(i < amountToBuffer.Length) bufferAmount = amountToBuffer[i];

            else 

                bufferAmount = defaultBufferAmount;

            

            for ( var n:int=0; n<bufferAmount; n++)

            {

                var newObj:GameObject = Instantiate(objectPrefab) as GameObject;

                newObj.name = objectPrefab.name;

                PoolObject(newObj);

            }

            

            i++;

        }

	}

    /// <summary>

    /// Gets a new object for the name type provided.  If no object type exists or if onlypooled is true and there is no objects of that type in the pool

    /// then null will be returned.

    /// </summary>

    /// <returns>

    /// The object for type.

    /// </returns>

    /// <param name='objectType'>

    /// Object type.

    /// </param>

    /// <param name='onlyPooled'>

    /// If true, it will only return an object if there is one currently pooled.

    /// </param>

    public function  GetObjectForType ( objectType:String ):GameObject

    {
		if(pooledObjects == null)
		{
			initPool();
		}
        for(var i:int=0; i<objectPrefabs.Length; i++)

        {

            var prefab:GameObject = objectPrefabs[i];

            if(prefab.name == objectType)

            {

                if(pooledObjects[i].Count > 0)

                {

                    var pooledObject:GameObject = pooledObjects[i][0];

                    pooledObjects[i].RemoveAt(0);

                    pooledObject.transform.parent = null;

                    pooledObject.SetActiveRecursively(true);

                    

                    return pooledObject;

                    

                } else{
					var newObject:GameObject = Instantiate(objectPrefabs[i]) as GameObject;
					PoolObject(newObject);
                    return newObject;

                }

                

                break;

                

            }

        }

            

        //If we have gotten here either there was no object of the specified type or non were left in the pool with onlyPooled set to true

        return null;

    }

    

    /// <summary>

    /// Pools the object specified.  Will not be pooled if there is no prefab of that type.

    /// </summary>

    /// <param name='obj'>

    /// Object to be pooled.

    /// </param>

    public function PoolObject (  obj: GameObject)

    {

        for ( var i:int=0; i<objectPrefabs.Length; i++)

        {

            if(objectPrefabs[i].name == obj.name)

            {

                obj.SetActiveRecursively(false);

                obj.transform.parent = containerObject.transform;

                pooledObjects[i].Add(obj);

                return;

            }

        }

    }
