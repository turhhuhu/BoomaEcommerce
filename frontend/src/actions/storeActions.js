import { CALL_API } from "../middleware/api";
import { STORE_PRODUCTS_URL, STORE_URL } from "../utils/constants";
import * as StoreActionTypes from "./types/storeActionsTypes";

export function fetchStoreInfo(storeGuid) {
  return {
    [CALL_API]: {
      endpoint: STORE_URL.replace("{storeGuid}", storeGuid),
      authenticated: true,
      types: [
        StoreActionTypes.STORE_INFO_REQUEST,
        StoreActionTypes.STORE_INFO_SUCCESS,
        StoreActionTypes.STORE_INFO_FAILURE,
      ],
    },
  };
}

export function fetchAllStoreProducts(storeGuid) {
  return {
    [CALL_API]: {
      endpoint: STORE_PRODUCTS_URL.replace("{storeGuid}", storeGuid),
      authenticated: true,
      types: [
        StoreActionTypes.GET_ALL_STORE_PRODUCTS_REQUEST,
        StoreActionTypes.GET_ALL_STORE_PRODUCTS_SUCCESS,
        StoreActionTypes.GET_ALL_STORE_PRODUCTS_FAILURE,
      ],
    },
  };
}

export function filterStoreProducts(filteredProducts) {
  return {
    type: StoreActionTypes.FILTER_STORE_PRODUCTS,
    payload: {
      filteredProducts: filteredProducts,
    },
  };
}
