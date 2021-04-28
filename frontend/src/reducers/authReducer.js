import * as AuthActionTypes from "../actions/types/authActionsTypes";
import { isValidJWT, extractUsernameFromJWT } from "../utils/jwtUtils";

// The auth reducer. The starting state sets authentication
// based on a token being in local storage. In a real app,
// we would also want a util to check if the token is expired.
export function auth(
  state = {
    isFetching: false,
    //TODO: need to add util function to check if token expired
    ...(isValidJWT()
      ? { username: extractUsernameFromJWT(), isAuthenticated: true }
      : { isAuthenticated: false }),
    error: undefined,
  },
  action
) {
  switch (action.type) {
    case AuthActionTypes.LOGIN_REQUEST:
      return Object.assign(
        {},
        state,
        Object.assign({ error: undefined }, action.payload)
      );
    case AuthActionTypes.LOGIN_SUCCESS:
      return Object.assign(
        {},
        state,
        Object.assign(
          {
            error: undefined,
            username: extractUsernameFromJWT(),
          },
          action.payload
        )
      );
    case AuthActionTypes.LOGIN_FAILURE:
      return Object.assign(
        {},
        state,
        Object.assign({ error: undefined }, action.payload)
      );
    case AuthActionTypes.LOGOUT_SUCCESS:
      return Object.assign(
        {},
        state,
        Object.assign({ error: undefined }, action.payload)
      );
    case AuthActionTypes.REGISTER_REQUEST:
      return Object.assign(
        {},
        state,
        Object.assign({ error: undefined }, action.payload)
      );
    case AuthActionTypes.REGISTER_SUCCESS:
      console.log(extractUsernameFromJWT());
      return Object.assign(
        {},
        state,
        Object.assign(
          { error: undefined, username: extractUsernameFromJWT() },
          action.payload
        )
      );
    case AuthActionTypes.REGISTER_FAILURE:
      return Object.assign(
        {},
        state,
        Object.assign({ error: undefined }, action.payload)
      );
    default:
      return state;
  }
}
