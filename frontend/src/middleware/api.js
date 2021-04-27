export const CALL_API = Symbol("Call API");

async function callApi(endpoint, authenticated) {
  let token = localStorage.getItem("access_token") || null;
  let config = {};

  if (authenticated) {
    // TODO: add token validation here
    if (token) {
      config = {
        headers: {
          Authorization: `bearer ${token}`,
        },
      };
    } else {
      throw new Error("no Token saved!");
    }
  }

  console.log(`${token}`);

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

  let { endpoint, types, authenticated } = callAPI;

  const [requestType, successType, errorType] = types;

  // Passing the authenticated boolean back in our data will let us distinguish between normal and secret quotes
  next({
    type: requestType,
  });

  return callApi(endpoint, authenticated).then(
    (response) =>
      next({
        payload: {
          response,
          authenticated,
        },
        type: successType,
      }),
    (error) =>
      next({
        error: error.message || "There was an error.",
        type: errorType,
      })
  );
};

export default middleware;
