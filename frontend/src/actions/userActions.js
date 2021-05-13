import { CALL_API } from "../middleware/api";
import { isValidJWT } from "../utils/jwtUtils";
import {
  USER_INFO_URL,
  USER_CART_URL,
  ADD_PRODUCT_TO_BASKET_URL,
  USER_CART_BASKETS_URL,
  PRODUCT_URL,
  STORE_URL,
  DELETE_PURCHASE_PRODUCT_FROM_BASKET_URL,
  USER_ROLES_URL,
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
  return {
    [CALL_API]: {
      endpoint: DELETE_PURCHASE_PRODUCT_FROM_BASKET_URL.replace(
        "{basketGuid}",
        basketGuid
      ).replace("{purchaseProductGuid}", purchaseProductGuid),
      authenticated: true,
      types: [
        UserActionTypes.REMOVE_PURCHASE_PRODUCT_FROM_BASKET_REQUEST,
        UserActionTypes.REMOVE_PURCHASE_PRODUCT_FROM_BASKET_SUCCESS,
        UserActionTypes.REMOVE_PURCHASE_PRODUCT_FROM_BASKET_FAILURE,
      ],
      config: {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
      },
      extraPayload: { basketGuid, purchaseProductGuid },
    },
  };
}

export function addProductToBasket(purchaseProduct) {
  let endpoint = ADD_PRODUCT_TO_BASKET_URL.replace(
    "{basketGuid}",
    purchaseProduct.basketGuid
  );
  return {
    [CALL_API]: {
      endpoint: endpoint,
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
  };
}

export function createBasketWithProduct(basket) {
  return {
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
  };
}
