import { combineReducers } from "redux";
import { auth } from "./authReducer";
import { user } from "./userReducer";
import { products } from "./productsReducer";
import { store } from "./storeReducer";

const combinedReducers = combineReducers({
  auth,
  user,
  products,
  store,
});

export default combinedReducers;
