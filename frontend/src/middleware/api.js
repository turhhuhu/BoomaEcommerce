// middleware/api.js
import { BASE_URL } from "../utils/constants";

async function callApi(endpoint, authenticated) {
  let token = localStorage.getItem("access_token") || null;
  let config = {};

  if (authenticated) {
    if (token) {
      config = {
        headers: { Authorization: `Bearer ${token}` },
      };
    } else {
      throw new Error("no Token saved!");
    }
  }

  return await fetch(BASE_URL + endpoint, config)
    .then(
      async (response) =>
        await response.json().then((text) => ({ text, response }))
    )
    .then(({ text, response }) => {
      if (!response.ok) {
        return Promise.reject(text);
      }

      return text;
    })
    .catch((err) => console.log(err));
}

export const CALL_API = Symbol("Call API");

const middleware = (store) => (next) => (action) => {
  const callAPI = action[CALL_API];

  // So the middleware doesn't get applied to every single action
  if (typeof callAPI === "undefined") {
    return next(action);
  }

  let { endpoint, types, authenticated } = callAPI;

  const [requestType, successType, errorType] = types;

  // Passing the authenticated boolean back in our data will let us distinguish between normal and secret quotes
  return callApi(endpoint, authenticated).then(
    (response) =>
      next({
        requestType: requestType,
        response,
        authenticated,
        type: successType,
      }),
    (error) =>
      next({
        requestType: requestType,
        error: error.message || "There was an error.",
        type: errorType,
      })
  );
};

export default middleware;
