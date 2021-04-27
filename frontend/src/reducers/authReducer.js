import * as AuthActionTypes from "../actions/types/authActionsTypes";

// The auth reducer. The starting state sets authentication
// based on a token being in local storage. In a real app,
// we would also want a util to check if the token is expired.
export function auth(
  state = {
    isFetching: false,
    //TODO: need to add util function to check if token expired
    isAuthenticated: localStorage.getItem("access_token") ? true : false,
  },
  action
) {
  switch (action.type) {
    case AuthActionTypes.LOGIN_REQUEST:
      return Object.assign({}, state, action.payload);
    case AuthActionTypes.LOGIN_SUCCESS:
      return Object.assign({}, state, action.payload);
    case AuthActionTypes.LOGIN_FAILURE:
      return Object.assign({}, state, action.payload);
    case AuthActionTypes.LOGOUT_SUCCESS:
      return Object.assign({}, state, action.payload);
    case AuthActionTypes.REGISTER_REQUEST:
      return Object.assign({}, state, action.payload);
    case AuthActionTypes.REGISTER_SUCCESS:
      return Object.assign({}, state, action.payload);
    case AuthActionTypes.REGISTER_FAILURE:
      return Object.assign({}, state, action.payload);
    default:
      return state;
  }
}
