import * as URLS from "../utils/constants";
import * as AuthActionTypes from "./types/authActionsTypes";

function requestLogin(creds) {
  return {
    type: AuthActionTypes.LOGIN_REQUEST,
    payload: {
      isFetching: true,
      isAuthenticated: false,
      username: creds.username,
    },
  };
}

function receiveLogin(responsePayload) {
  return {
    type: AuthActionTypes.LOGIN_SUCCESS,
    payload: {
      isFetching: false,
      isAuthenticated: true,
      user_guid: responsePayload.userGuid,
      access_token: responsePayload.token,
    },
  };
}

function loginError(message) {
  return {
    type: AuthActionTypes.LOGIN_FAILURE,
    payload: {
      isFetching: false,
      isAuthenticated: false,
      message,
    },
  };
}

export function loginUser(creds) {
  let config = {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      username: creds.username,
      password: creds.password,
    }),
  };
  return async (dispatch) => {
    // We dispatch requestLogin to kickoff the call to the API
    dispatch(requestLogin(creds));
    return await fetch(URLS.LOGIN_URL, config)
      .then(
        async (response) =>
          await response
            .json()
            .then((responsePayload) => ({ responsePayload, response }))
      )
      .then(({ responsePayload, response }) => {
        if (!response.ok) {
          // If there was a problem, we want to
          // dispatch the error condition
          dispatch(loginError(responsePayload.message));
          return Promise.reject(responsePayload);
        } else {
          // If login was successful, set the token in local storage
          localStorage.setItem("user_guid", responsePayload.userGuid);
          localStorage.setItem("access_token", responsePayload.token);
          localStorage.setItem("refresh_token", responsePayload.refreshToken);
          // Dispatch the success action
          dispatch(receiveLogin(responsePayload));
        }
      })
      .catch((err) => console.log("Error: ", err));
  };
}

function requestRegister(userInfo) {
  return {
    type: AuthActionTypes.REGISTER_REQUEST,
    payload: {
      isFetching: true,
      isAuthenticated: false,
      username: userInfo.userName,
    },
  };
}

function receiveRegister(responsePayload) {
  return {
    type: AuthActionTypes.REGISTER_SUCCESS,
    payload: {
      isFetching: false,
      isAuthenticated: true,
      user_guid: responsePayload.userGuid,
      access_token: responsePayload.token,
    },
  };
}

function RegisterError(message) {
  return {
    type: AuthActionTypes.REGISTER_FAILURE,
    payload: {
      isFetching: false,
      isAuthenticated: false,
      message,
    },
  };
}

export function RegisterUser(userInfo) {
  let config = {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      userInfo: {
        userName: userInfo.username,
        name: userInfo.name,
        lastName: userInfo.lastName,
      },
      password: userInfo.password,
    }),
  };
  return async (dispatch) => {
    // We dispatch requestLogin to kickoff the call to the API
    dispatch(requestRegister(userInfo));
    return await fetch(URLS.REGISTER_URL, config)
      .then(
        async (response) =>
          await response
            .json()
            .then((responsePayload) => ({ responsePayload, response }))
      )
      .then(({ responsePayload, response }) => {
        console.log(JSON.stringify(responsePayload));
        console.log(response);
        if (!response.ok) {
          // If there was a problem, we want to
          // dispatch the error condition
          dispatch(RegisterError(responsePayload.message));
          return Promise.reject(responsePayload);
        } else {
          // Dispatch the success action
          dispatch(receiveRegister(responsePayload));
        }
      })
      .catch((err) => console.log("Error: ", err));
  };
}

function requestLogout() {
  return {
    type: AuthActionTypes.LOGOUT_REQUEST,
    payload: {
      isFetching: true,
      isAuthenticated: true,
    },
  };
}

function receiveLogout() {
  return {
    type: AuthActionTypes.LOGOUT_SUCCESS,
    payload: {
      isFetching: false,
      isAuthenticated: false,
    },
  };
}

export function logoutUser() {
  return (dispatch) => {
    dispatch(requestLogout());
    localStorage.removeItem("user_guid");
    localStorage.removeItem("access_token");
    localStorage.setItem("refresh_token");
    dispatch(receiveLogout());
  };
}
