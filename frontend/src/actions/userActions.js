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
  CREATE_PURCHASE_URL,
  GET_CART_DISCOUNTED_PRICE_URL,
  USER_PURCHASE_HISTORY_URL,
  PRODUCT_PRICE_OFFER_URL,
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
export function receiveStorePurchaseNotification(notification) {
  return {
    type: UserActionTypes.RECEIVE_STORE_PURCHASE_NOTIFICATION,
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

export function submitPaymentInfo(paymentInfo) {
  return {
    type: UserActionTypes.SUBMIT_PAYMENT_INFO,
    payload: {
      paymentInfo,
    },
  };
}

export function submitDeliveryInfo(deliveryInfo) {
  return {
    type: UserActionTypes.SUBMIT_DELIVERY_INFO,
    payload: {
      deliveryInfo,
    },
  };
}

export function createPurchase(purchaseDetails) {
  let token = localStorage.getItem("access_token") || null;
  return {
    [CALL_API]: {
      endpoint: CREATE_PURCHASE_URL,
      authenticated: token ? true : false,
      types: [
        UserActionTypes.CREATE_PURCHASE_REQUEST,
        UserActionTypes.CREATE_PURCHASE_SUCCESS,
        UserActionTypes.CREATE_PURCHASE_FAILURE,
      ],
      config: {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(purchaseDetails),
      },
    },
  };
}

export function fetchCartDiscountedPrice(cartAsPurchase) {
  let token = localStorage.getItem("access_token") || null;
  return {
    [CALL_API]: {
      endpoint: GET_CART_DISCOUNTED_PRICE_URL,
      authenticated: token ? true : false,
      types: [
        UserActionTypes.GET_CART_DISCOUNTED_PRICE_REQUEST,
        UserActionTypes.GET_CART_DISCOUNTED_PRICE_SUCCESS,
        UserActionTypes.GET_CART_DISCOUNTED_PRICE_FAILURE,
      ],
      config: {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(cartAsPurchase),
      },
    },
  };
}

export function submitGuestInformation(guestInformation) {
  return {
    type: UserActionTypes.SUBMIT_GUEST_INFORMATION,
    payload: {
      guestInformation,
    },
  };
}

export function clearGuestCart() {
  return {
    type: UserActionTypes.CLEAR_GUEST_CART,
  };
}

export function fetchUserPurchaseHistory() {
  return {
    [CALL_API]: {
      endpoint: USER_PURCHASE_HISTORY_URL,
      authenticated: true,
      types: [
        UserActionTypes.GET_PURCHASE_HISTORY_REQUEST,
        UserActionTypes.GET_PURCHASE_HISTORY_SUCCESS,
        UserActionTypes.GET_PURCHASE_HISTORY_FAILURE,
      ],
    },
  };
}

export function offerProductPrice(offer) {
  return {
    [CALL_API]: {
      endpoint: PRODUCT_PRICE_OFFER_URL,
      authenticated: true,
      types: [
        UserActionTypes.OFFER_PRODUCT_PRICE_REQUEST,
        UserActionTypes.OFFER_PRODUCT_PRICE_SUCCESS,
        UserActionTypes.OFFER_PRODUCT_PRICE_FAILURE,
      ],
      config: {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(offer),
      },
    },
  };
}

export function fetchProductOffers() {
  return {
    [CALL_API]: {
      endpoint: PRODUCT_PRICE_OFFER_URL,
      authenticated: true,
      types: [
        UserActionTypes.GET_PRODUCT_OFFERS_REQUEST,
        UserActionTypes.GET_PRODUCT_OFFERS_SUCCESS,
        UserActionTypes.GET_PRODUCT_OFFERS_FAILURE,
      ],
    },
  };
}
