import "bootstrap/dist/css/bootstrap.css";
import "font-awesome/css/font-awesome.min.css";
import { createStore, applyMiddleware, compose } from "redux";
import thunkMiddleware from "redux-thunk";
import api from "./middleware/api";
import combinedReducers from "./reducers/combinedReducers";
import { persistStore, persistReducer } from "redux-persist";
import storage from "redux-persist/lib/storage";
import reduxReset from "redux-reset";

function configureStore() {
  const createStoreWithMiddleware = compose(
    applyMiddleware(thunkMiddleware, api),
    reduxReset()
  )(createStore);

  const persistConfig = {
    key: "root",
    storage,
  };

  const persistedReducer = persistReducer(persistConfig, combinedReducers);

  const store = createStoreWithMiddleware(
    persistedReducer,
    window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__()
  );

  const persistor = persistStore(store);
  return { store, persistor };
}

export default configureStore;
