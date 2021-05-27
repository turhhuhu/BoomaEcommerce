//import { CALL_API } from "../middleware/api";
import { GET_ALL_PRODUCTS_URL, STORE_URL } from "../utils/constants";
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

    let productsPromises = response.map((product) =>
      fetch(STORE_URL.replace("{storeGuid}", product.storeGuid))
        .then((response) => response.json())
        .then((store) => (product["store"] = store))
    );

    await Promise.all(productsPromises).catch((error) =>
      dispatch(productsError(error))
    );
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

function productsError(error) {
  return {
    type: ProductsActionTypes.GET_ALL_PRODUCTS_FAILURE,
    payload: {
      isFetching: false,
    },
    error,
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
