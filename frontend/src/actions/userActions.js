import { CALL_API } from "../middleware/api";
import { isValidJWT } from "../utils/jwtUtils";
import {
  USER_INFO_URL,
  USER_CART_URL,
  ADD_PRODUCT_TO_BASKET_URL,
  USER_CART_BASKETS_URL,
  PRODUCT_URL,
  STORE_URL,
  DELETE_PRODUCT_FROM_BASKET_URL,
  USER_ROLES_URL,
  ADD_STORE_URL,
  USER_STORE_ROLE,
  SEE_NOTIFICATION_URL,
} from "../utils/constants";
import * as UserActionTypes from "./types/userActionsTypes";

export function fetchUserInfo() {
  return {
    [CALL_API]: {
      endpoint: USER_INFO_URL,
      authenticated: true,
      types: [
        UserActionTypes.USER_INFO_REQUEST,
        UserActionTypes.USER_INFO_SUCCESS,
        UserActionTypes.USER_INFO_FAILURE,
      ],
    },
  };
}

export function fetchUserRoles() {
  return {
    [CALL_API]: {
      endpoint: USER_ROLES_URL,
      authenticated: true,
      types: [
        UserActionTypes.USER_ROLES_REQUEST,
        UserActionTypes.USER_ROLES_SUCCESS,
        UserActionTypes.USER_ROLES_FAILURE,
      ],
    },
  };
}

export function fetchUserCart() {
  let token = localStorage.getItem("access_token") || null;
  let config = {};
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
    console.error("not token saved!");
  }

  return async (dispatch) => {
    dispatch(requestCart());
    if (!token) {
      dispatch(cartError("not logged in"));
      return;
    }
    let response = await fetch(USER_CART_URL, config)
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

    let storesPromises = response.baskets.map((basket) =>
      fetch(STORE_URL.replace("{storeGuid}", basket.storeGuid))
        .then((response) => response.json())
        .then((store) => (basket["store"] = store))
    );

    await Promise.all(storesPromises).catch((error) =>
      dispatch(cartError(error))
    );

    let productsPromises = response.baskets
      .flatMap((basket) => {
        return basket.purchaseProducts;
      })
      .map((purchaseProduct) =>
        fetch(PRODUCT_URL.replace("{productGuid}", purchaseProduct.productGuid))
          .then((response) => response.json())
          .then((product) => (purchaseProduct["product"] = product))
      );
    await Promise.all(productsPromises).catch((error) =>
      dispatch(cartError(error))
    );

    dispatch(receiveCart(response));
  };
}

function requestCart() {
  return {
    type: UserActionTypes.USER_CART_REQUEST,
    payload: {
      isFetching: true,
    },
  };
}

function receiveCart(response) {
  return {
    type: UserActionTypes.USER_CART_SUCCESS,
    payload: {
      isFetching: false,
      response: response,
    },
  };
}

function cartError(error) {
  return {
    type: UserActionTypes.USER_CART_FAILURE,
    payload: {
      isFetching: false,
    },
    error,
  };
}

export function removeCartItem(basketGuid, purchaseProductGuid) {
  return (dispatch, getState) => {
    if (getState().auth.isAuthenticated) {
      dispatch({
        [CALL_API]: {
          endpoint: DELETE_PRODUCT_FROM_BASKET_URL.replace(
            "{basketGuid}",
            basketGuid
          ).replace("{purchaseProductGuid}", purchaseProductGuid),
          authenticated: true,
          types: [
            UserActionTypes.REMOVE_PRODUCT_FROM_BASKET_REQUEST,
            UserActionTypes.REMOVE_PRODUCT_FROM_BASKET_SUCCESS,
            UserActionTypes.REMOVE_PRODUCT_FROM_BASKET_FAILURE,
          ],
          config: {
            method: "DELETE",
            headers: { "Content-Type": "application/json" },
          },
          extraPayload: { basketGuid, purchaseProductGuid },
        },
      });
    } else {
      dispatch({
        type: UserActionTypes.REMOVE_PRODUCT_FROM_BASKET_AS_GUEST,
        payload: {
          basketGuid,
          purchaseProductGuid,
        },
      });
    }
  };
}

export function changeCartProductAmount(
  basketGuid,
  purchaseProductGuid,
  newAmount
) {
  return {
    type: UserActionTypes.CHANGE_PRODUCT_AMOUNT_IN_BASKET,
    payload: {
      basketGuid,
      purchaseProductGuid,
      newAmount,
    },
  };
}

export function addProductToBasket(purchaseProduct, product) {
  return (dispatch, getState) => {
    if (getState().auth.isAuthenticated) {
      dispatch({
        [CALL_API]: {
          endpoint: ADD_PRODUCT_TO_BASKET_URL.replace(
            "{basketGuid}",
            purchaseProduct.basketGuid
          ),
          authenticated: true,
          types: [
            UserActionTypes.ADD_PRODUCT_TO_BASKET_REQUEST,
            UserActionTypes.ADD_PRODUCT_TO_BASKET_SUCCESS,
            UserActionTypes.ADD_PRODUCT_TO_BASKET_FAILURE,
          ],
          config: {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(purchaseProduct.purchaseProduct),
          },
          extraPayload: purchaseProduct.basketGuid,
        },
      });
    } else {
      purchaseProduct.purchaseProduct["product"] = product;
      dispatch({
        type: UserActionTypes.ADD_PRODUCT_TO_BASKET_AS_GUEST,
        payload: {
          basketGuid: purchaseProduct.basketGuid,
          purchaseProduct: purchaseProduct,
        },
      });
    }
  };
}

export function createBasketWithProduct(basket, product) {
  return (dispatch, getState) => {
    if (getState().auth.isAuthenticated) {
      dispatch({
        [CALL_API]: {
          endpoint: USER_CART_BASKETS_URL,
          authenticated: true,
          types: [
            UserActionTypes.ADD_PRODUCT_WITH_BASKET_REQUEST,
            UserActionTypes.ADD_PRODUCT_WITH_BASKET_SUCCESS,
            UserActionTypes.ADD_PRODUCT_WITH_BASKET_FAILURE,
          ],
          config: {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(basket),
          },
        },
      });
    } else {
      basket.purchaseProducts[0]["product"] = product;
      dispatch({
        type: UserActionTypes.ADD_PRODUCT_WITH_BASKET_AS_GUEST,
        payload: {
          basket,
        },
      });
    }
  };
}

export function addStore(store) {
  return {
    [CALL_API]: {
      endpoint: ADD_STORE_URL,
      authenticated: true,
      types: [
        UserActionTypes.ADD_STORE_REQUEST,
        UserActionTypes.ADD_STORE_SUCCESS,
        UserActionTypes.ADD_STORE_FAILURE,
      ],
      config: {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(store),
      },
    },
  };
}

export function fetchUserStoreRole(storeGuid) {
  return {
    [CALL_API]: {
      endpoint: USER_STORE_ROLE.replace("{storeGuid}", storeGuid),
      authenticated: true,
      types: [
        UserActionTypes.USER_STORE_ROLE_REQUEST,
        UserActionTypes.USER_STORE_ROLE_SUCCESS,
        UserActionTypes.USER_STORE_ROLE_FAILURE,
      ],
    },
  };
}

export function StartWebSocketConnection(webSocketConnection) {
  return {
    type: UserActionTypes.START_WEB_SOCKET_CONNECTION,
    payload: { webSocketConnection: webSocketConnection },
  };
}

export function CloseWebSocketConnection() {
  return (dispatch, getState) => {
    const webSocketConnection = getState().user.webSocketConnection;
    webSocketConnection.stop();
    dispatch({
      type: UserActionTypes.CLOSE_WEB_SOCKET_CONNECTION,
    });
  };
}

export function receiveRegularNotification(notification) {
  return {
    type: UserActionTypes.RECIEVE_REGULAR_NOTIFICATION,
    payload: {
      notification,
    },
  };
}

export function receiveRoleDismissalNotification(notification) {
  return {
    type: UserActionTypes.RECIEVE_ROLE_DISMISSAL_NOTIFICATION,
    payload: {
      notification,
    },
  };
}

export function seeNotification(notificationGuid) {
  return async (dispatch, getState) => {
    const seenNotificationIndex = getState().user.notifications.findIndex(
      (notification) => notification.guid === notificationGuid
    );
    if (seenNotificationIndex === -1) {
      return;
    }
    let seenNotification = getState().user.notifications[seenNotificationIndex];
    seenNotification.wasSeen = true;
    let token = localStorage.getItem("access_token") || null;
    let config = {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
    };
    if (token) {
      if (!isValidJWT(token)) {
        throw new Error("unvalid Token!");
      }
      config["headers"] = {
        authorization: `bearer ${token}`,
      };
    } else {
      console.error("not token saved!");
      return;
    }

    await fetch(
      SEE_NOTIFICATION_URL.replace("{notificationGuid}", notificationGuid),
      config
    ).then(async (response) => {
      if (response.status === 204) {
        dispatch({
          type: UserActionTypes.SEE_NOTIFICATION,
          payload: {
            seenNotificationIndex,
            seenNotification,
          },
        });
      }
    });
  };
}
