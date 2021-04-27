import { CALL_API } from "../middleware/api";
import { GET_ALL_PRODUCTS_URL } from "../utils/constants";
import * as ProductsActionTypes from "./types/productsActionsTypes";

export function fetchAllProducts() {
  return {
    [CALL_API]: {
      endpoint: GET_ALL_PRODUCTS_URL,
      authenticated: true,
      types: [
        ProductsActionTypes.GET_ALL_PRODUCTS_REQUEST,
        ProductsActionTypes.GET_ALL_PRODUCTS_SUCCESS,
        ProductsActionTypes.GET_ALL_PRODUCTS_FAILURE,
      ],
    },
  };
}
