import * as UserActionTypes from "../actions/types/userActionsTypes";
export function user(
  state = {
    isFetching: false,
    userInfo: {},
    authenticated: true,
  },
  action
) {
  switch (action.type) {
    case UserActionTypes.USER_INFO_REQUEST:
      return state;
    case UserActionTypes.USER_INFO_SUCCESS:
      console.log(action);
      return Object.assign({}, state, {
        userInfo: action.payload.response,
      });
    case UserActionTypes.USER_INFO_FAILURE:
      console.error(`error occured while getting user info: ${action.error}`);
      return state;
    default:
      return state;
  }
}
