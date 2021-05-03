export const BASE_URL = "https://localhost:5001/api";

//authentication
export const AUTHENTICATION_URL = BASE_URL + "/Authentication";
export const LOGIN_URL = AUTHENTICATION_URL + "/login";
export const REGISTER_URL = AUTHENTICATION_URL + "/register";

//user
export const USER_URL = BASE_URL + "/Users";
export const USER_INFO_URL = USER_URL + "/me";
export const USER_CART_URL = USER_INFO_URL + "/cart";
export const USER_CART_BASKETS_URL = USER_CART_URL + "/baskets";
export const USER_CART_BASKET_URL = USER_CART_BASKETS_URL + "/{basketGuid}";
export const ADD_PRODUCT_TO_BASKET_URL = USER_CART_BASKET_URL + "/products";
export const USER_CART_BASKET_PRODUCTS_URL = USER_CART_BASKET_URL + "/products";
export const DELETE_PURCHASE_PRODUCT_FROM_BASKET_URL =
  USER_CART_BASKET_PRODUCTS_URL + "/{purchaseProductGuid}";

//products
export const PRODUCTS_URL = BASE_URL + "/Products";
export const PRODUCT_URL = PRODUCTS_URL + "/{productGuid}";
export const GET_ALL_PRODUCTS_URL = PRODUCTS_URL;

//stores
export const STORES_URL = BASE_URL + "/Stores";
export const STORE_URL = STORES_URL + "/{storeGuid}";
