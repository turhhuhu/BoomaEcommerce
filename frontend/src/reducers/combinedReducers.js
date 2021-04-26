import { combineReducers } from "redux";
import { auth } from "./authReducer";
const combinedReducers = combineReducers({
  auth,
});

export default combinedReducers;
