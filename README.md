
# BoomaEcommerce 

## Init File

The Init file is a JSON file that has a list of Actions that will run one after another before the system goes up.  
to add a new action to the init file, you need to check its Use case Action class and find what arguments it should get.  
each use case has a Label parameter, which represent the 'returned value' to pass through to the next action.



### For Example


```json
 {
        "Type": "CreateStoreAction",
        "UserLabel": "Benny User",
        "Label" : "Rami Levi",
        "StoreToCreate": 
        {
           "StoreName": "Rami Levi",
           "Description": "Blah"
        }
  }
```

create store action need to get User label which represent the user that want to open a store, Label which will direct any future action to this store and some new store details. 





workshop in software engineering project
Matan Hazan 315198796
Omer Kempner 322217472
Ori Kintzlinger 318929213
Arye Shapiro 313578379 
Benny Skidanov 322572926