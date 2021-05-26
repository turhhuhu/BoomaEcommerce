export const BASE_URL = "https://localhost:5001/api";
export const NOTIFICATION_HUB_URL = "http://localhost:5000/hub/notifications";

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
export const USER_STORES_URL = USER_INFO_URL + "/stores";
export const USER_ROLES_URL = USER_STORES_URL + "/allRoles";
export const ADD_STORE_URL = USER_STORES_URL;
export const USER_STORE_ROLE = USER_STORES_URL + "/{storeGuid}/role";

//products
export const PRODUCTS_URL = BASE_URL + "/Products";
export const PRODUCT_URL = PRODUCTS_URL + "/{productGuid}";
export const GET_ALL_PRODUCTS_URL = PRODUCTS_URL;

//stores
export const STORES_URL = BASE_URL + "/Stores";
export const STORE_URL = STORES_URL + "/{storeGuid}";
export const STORE_PRODUCTS_URL = STORE_URL + "/products";
export const STORE_PRODUCT_URL = STORE_PRODUCTS_URL + "/{productGuid}";
export const STORE_ROLES_URL = STORE_URL + "/roles";
export const STORE_SUBORDINATES_URL =
  STORE_ROLES_URL + "/ownerships/{ownershipGuid}/subordinates";
export const ADD_STORE_OWNER_URL = STORE_ROLES_URL + "/ownerships";
export const ADD_STORE_MANAGER_URL = STORE_ROLES_URL + "/managements";

//roles
export const ROLES_URL = BASE_URL + "/Roles";
export const MANAGER_ROLE_URL = ROLES_URL + "/managements/{managementGuid}";
export const OWNER_ROLE_URL = ROLES_URL + "/ownerships/{ownershipGuid}";
export const SUBORDINATES_URL = OWNER_ROLE_URL + "/subordinates";
export const EDIT_MANAGER_PERMISSIONS_URL = MANAGER_ROLE_URL + "/permissions";
export const REMOVE_SUBORDINATE_URL =
  SUBORDINATES_URL + "/{roleToDeleteGuid}?roleType={roleType}";
