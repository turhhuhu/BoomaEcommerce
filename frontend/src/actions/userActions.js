import { CALL_API } from "../middleware/api";
import { USER_INFO_URL } from "../utils/constants";
import * as UserActionTypes from "./types/userActionsTypes";

export function fetchUserInfo(isAuthenticated) {
  return {
    [CALL_API]: {
      endpoint: USER_INFO_URL,
      authenticated: isAuthenticated,
      types: [
        UserActionTypes.USER_INFO_REQUEST,
        UserActionTypes.USER_INFO_SUCCESS,
        UserActionTypes.USER_INFO_FAILURE,
      ],
    },
  };
}
