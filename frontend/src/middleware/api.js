import { isValidJWT } from "../utils/jwtUtils";

export const CALL_API = Symbol("Call API");

async function callApi(endpoint, authenticated, config) {
  let token = localStorage.getItem("access_token") || null;
  if (!config) {
    config = {};
  }

  if (authenticated) {
    if (token) {
      if (!isValidJWT(token)) {
        throw new Error("unvalid Token!");
      }
      if (config.headers) {
        config["headers"] = {
          ...config.headers,
          authorization: `bearer ${token}`,
        };
      } else {
        config["headers"] = {
          authorization: `bearer ${token}`,
        };
      }
    } else {
      throw new Error("no Token saved!");
    }
  }

  return fetch(endpoint, config)
    .then((response) => {
      if (response.status === 204) {
        return { responsePayLoad: null, response };
      }
      return response
        .json()
        .then((responsePayLoad) => ({ responsePayLoad, response }));
    })
    .then(({ responsePayLoad, response }) => {
      if (!response.ok) {
        return Promise.reject(responsePayLoad);
      }

      return responsePayLoad;
    })
    .catch((err) => Promise.reject(err));
}

const middleware = (store) => (next) => (action) => {
  const callAPI = action[CALL_API];

  // So the middleware doesn't get applied to every single action
  if (typeof callAPI === "undefined") {
    return next(action);
  }

  let { endpoint, types, authenticated, config, extraPayload } = callAPI;

  const [requestType, successType, errorType] = types;

  // Passing the authenticated boolean back in our data will let us distinguish between normal and secret quotes
  next({
    type: requestType,
    payload: { isFetching: true },
  });

  return callApi(endpoint, authenticated, config).then(
    (response) =>
      next({
        payload: {
          response,
          authenticated,
          isFetching: false,
        },
        type: successType,
        extraPayload: extraPayload,
      }),
    (error) => {
      next({
        error: error.message || "There was an error.",
        payload: {
          isFetching: false,
        },
        type: errorType,
      });
    }
  );
};

export default middleware;
