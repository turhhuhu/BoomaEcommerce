import * as URLS from "../utils/constants";
import * as AuthActionTypes from "./types/authActionsTypes";
import storage from "redux-persist/lib/storage";
import { fetchUserCart } from "./userActions";

function requestLogin() {
  return {
    type: AuthActionTypes.LOGIN_REQUEST,
    payload: {
      isFetching: true,
      isAuthenticated: false,
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
    },
  };
}

function loginError(error) {
  return {
    type: AuthActionTypes.LOGIN_FAILURE,
    payload: {
      isFetching: false,
      isAuthenticated: false,
      error,
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
    let gotResponse = false;
    return await fetch(URLS.LOGIN_URL, config)
      .then(
        async (response) =>
          await response
            .json()
            .then((responsePayload) => ({ responsePayload, response }))
      )
      .then(({ responsePayload, response }) => {
        gotResponse = true;
        if (!response.ok) {
          // If there was a problem, we want to
          // dispatch the error condition
          let errors = undefined;
          if (responsePayload.errors)
            errors = [
              responsePayload.errors.Username,
              responsePayload.errors.Password,
            ].join("\n");
          else errors = responsePayload;
          dispatch(loginError(errors));
          return Promise.reject(responsePayload);
        } else {
          // If login was successful, set the token in local storage
          localStorage.setItem("access_token", responsePayload.token);
          localStorage.setItem("refresh_token", responsePayload.refreshToken);
          // Dispatch the success action
          dispatch(receiveLogin({ responsePayload }));
          dispatch(fetchUserCart());
        }
      })
      .catch((err) => {
        if (!gotResponse) {
          dispatch(loginError("API connection failure"));
        }
        console.error("Error: ", err);
      });
  };
}

function requestRegister() {
  return {
    type: AuthActionTypes.REGISTER_REQUEST,
    payload: {
      isFetching: true,
      isAuthenticated: false,
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
    },
  };
}

function RegisterError(error) {
  return {
    type: AuthActionTypes.REGISTER_FAILURE,
    payload: {
      isFetching: false,
      isAuthenticated: false,
      error,
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
    dispatch(requestRegister());
    let gotResponse = false;
    return await fetch(URLS.REGISTER_URL, config)
      .then(
        async (response) =>
          await response
            .json()
            .then((responsePayload) => ({ responsePayload, response }))
      )
      .then(({ responsePayload, response }) => {
        gotResponse = true;
        if (!response.ok) {
          // If there was a problem, we want to
          // dispatch the error condition
          dispatch(RegisterError(responsePayload));
          return Promise.reject(responsePayload);
        } else {
          // Dispatch the success action
          localStorage.setItem("access_token", responsePayload.token);
          localStorage.setItem("refresh_token", responsePayload.refreshToken);
          dispatch(receiveRegister(responsePayload));
        }
      })
      .catch((err) => {
        if (!gotResponse) {
          dispatch(loginError("API connection failure"));
        }
        console.error("Error: ", err);
      });
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
    dispatch({
      type: "RESET",
    });
    localStorage.removeItem("access_token");
    localStorage.removeItem("refresh_token");
    storage.removeItem("persist:root");
    dispatch(receiveLogout());
  };
}
