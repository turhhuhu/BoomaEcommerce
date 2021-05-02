import { combineReducers } from "redux";
import { auth } from "./authReducer";
import { user } from "./userReducer";
import { products } from "./productsReducer";

const combinedReducers = combineReducers({
  auth,
  user,
  products,
});

export default combinedReducers;
