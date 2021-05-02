import { CALL_API } from "../middleware/api";
import {
  USER_INFO_URL,
  USER_CART_URL,
  ADD_PRODUCT_TO_BASKET_URL,
  USER_CART_BASKETS_URL,
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

export function fetchUserCart() {
  return {
    [CALL_API]: {
      endpoint: USER_CART_URL,
      authenticated: true,
      types: [
        UserActionTypes.USER_CART_REQUEST,
        UserActionTypes.USER_CART_SUCCESS,
        UserActionTypes.USER_CART_FAILURE,
      ],
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
