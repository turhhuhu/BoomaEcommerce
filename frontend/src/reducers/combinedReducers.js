import { combineReducers } from "redux";
import { auth } from "./authReducer";
import { user } from "./userReducer";
import { productsView } from "./productsReducer";
const combinedReducers = combineReducers({
  auth,
  user,
  productsView,
});

export default combinedReducers;
