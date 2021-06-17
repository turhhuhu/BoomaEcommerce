import { GET_ALL_PRODUCTS_URL } from "../utils/constants";
import * as ProductsActionTypes from "./types/productsActionsTypes";

export function fetchAllProducts(searchFilter) {
  let getProductsUrl = GET_ALL_PRODUCTS_URL;
  if (searchFilter) {
    const searchWord = Object.keys(searchFilter)[0];
    getProductsUrl += `?${searchWord}=${searchFilter[searchWord]}`;
    console.log(getProductsUrl);
  }

  return async (dispatch) => {
    const config = {};
    dispatch(requestProducts());
    let response = await fetch(getProductsUrl, config)
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

    dispatch(receiveProducts(response));
  };
}

function requestProducts() {
  return {
    type: ProductsActionTypes.GET_ALL_PRODUCTS_REQUEST,
    payload: {
      isFetching: true,
    },
  };
}

function receiveProducts(response) {
  return {
    type: ProductsActionTypes.GET_ALL_PRODUCTS_SUCCESS,
    payload: {
      isFetching: false,
      response: response,
    },
  };
}

export function filterProducts(filteredProducts) {
  return {
    type: ProductsActionTypes.FILTER_PRODUCTS,
    payload: {
      filteredProducts: filteredProducts,
    },
  };
}
