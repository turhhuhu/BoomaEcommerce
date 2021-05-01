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
      config["headers"] = {
        Authorization: `bearer ${token}`,
      };
    } else {
      throw new Error("no Token saved!");
    }
  }

  console.log(config);

  return await fetch(endpoint, config)
    .then(
      async (response) =>
        await response
          .json()
          .then((responsePayLoad) => ({ responsePayLoad, response }))
    )
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
  });

  return callApi(endpoint, authenticated, config).then(
    (response) =>
      next({
        payload: {
          response,
          authenticated,
        },
        type: successType,
        extraPayload: extraPayload,
      }),
    (error) =>
      next({
        error: error.message || "There was an error.",
        type: errorType,
      })
  );
};

export default middleware;
