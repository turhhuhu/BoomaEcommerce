import { isValidJWT } from "../utils/jwtUtils";

export const CALL_API = Symbol("Call API");

async function callApi(endpoint, authenticated, config) {
  let token = localStorage.getItem("access_token") || null;
  if (!config) {
    config = {};
  } else {
    console.log(config);
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
        .text()
        .then((text) => {
          const errorText = text.substring(1, text.length - 1);
          if (response.status === 400 && !errorText.startsWith('"type":')) {
            if (errorText.startsWith("{")) {
              const error = JSON.parse(text);
              const errorMsg = error
                .map((errorObj) => errorObj.error)
                .join("\n");
              return Promise.reject(errorMsg);
            }
            return Promise.reject(errorText);
          }
          if (response.status === 500) {
            return Promise.reject(errorText);
          }
          return text ? JSON.parse(text) : {};
        })
        .then((responsePayLoad) => ({ responsePayLoad, response }));
    })
    .then(({ responsePayLoad, response }) => {
      if (!response.ok) {
        if (response.status === 400) {
          return Promise.reject("Bad request or unexpected error");
        }
        if (response.status === 401) {
          return Promise.reject("Unauthorized");
        }
        if (response.status === 404) {
          return Promise.reject("Not found");
        }
        if (response.status === 500) {
          return Promise.reject("Internal server error");
        }
        return responsePayLoad.errors
          ? Promise.reject(responsePayLoad.errors[""][0])
          : Promise.reject(responsePayLoad.join("\n"));
      }

      return responsePayLoad;
    })
    .catch((err) => {
      return Promise.reject(err);
    });
}

const middleware = (store) => (next) => (action) => {
  if (action === undefined) {
    return next({ type: "UNDEFINED_ACTION" });
  }
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
        error: error || "There was an error.",
        payload: {
          isFetching: false,
        },
        type: errorType,
      });
    }
  );
};

export default middleware;
