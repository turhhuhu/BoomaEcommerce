import { CALL_API } from "../middleware/api";
import {
  STORE_PRODUCTS_URL,
  STORE_PRODUCT_URL,
  STORE_ROLES_URL,
  STORE_URL,
  STORE_SUBORDINATES_URL,
  ADD_STORE_OWNER_URL,
  ADD_STORE_MANAGER_URL,
  REMOVE_SUBORDINATE_URL,
  STORE_SUB_POLICIES_URL,
  STORE_POLICIES_URL,
  STORE_POLICY_URL,
} from "../utils/constants";
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

export function addProductToStore(product, storeGuid) {
  return {
    [CALL_API]: {
      endpoint: STORE_PRODUCTS_URL.replace("{storeGuid}", storeGuid),
      authenticated: true,
      types: [
        StoreActionTypes.ADD_PRODUCT_TO_STORE_REQUEST,
        StoreActionTypes.ADD_PRODUCT_TO_STORE_SUCCESS,
        StoreActionTypes.ADD_PRODUCT_TO_STORE_FAILURE,
      ],
      config: {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(product),
      },
    },
  };
}

export function removeStoreProduct(storeGuid, productGuid) {
  return {
    [CALL_API]: {
      endpoint: STORE_PRODUCT_URL.replace("{storeGuid}", storeGuid).replace(
        "{productGuid}",
        productGuid
      ),
      authenticated: true,
      types: [
        StoreActionTypes.REMOVE_PRODUCT_FROM_STORE_REQUEST,
        StoreActionTypes.REMOVE_PRODUCT_FROM_STORE_SUCCESS,
        StoreActionTypes.REMOVE_PRODUCT_FROM_STORE_FAILURE,
      ],
      config: {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
      },
      extraPayload: { productGuid },
    },
  };
}

export function editStoreProduct(storeGuid, productGuid, editedProduct) {
  return {
    [CALL_API]: {
      endpoint: STORE_PRODUCT_URL.replace("{storeGuid}", storeGuid).replace(
        "{productGuid}",
        productGuid
      ),
      authenticated: true,
      types: [
        StoreActionTypes.EDIT_PRODUCT_FROM_STORE_REQUEST,
        StoreActionTypes.EDIT_PRODUCT_FROM_STORE_SUCCESS,
        StoreActionTypes.EDIT_PRODUCT_FROM_STORE_FAILURE,
      ],
      config: {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(editedProduct),
      },
      extraPayload: { editedProduct, productGuid },
    },
  };
}

export function fetchStoreRoles(storeGuid) {
  return {
    [CALL_API]: {
      endpoint: STORE_ROLES_URL.replace("{storeGuid}", storeGuid),
      authenticated: true,
      types: [
        StoreActionTypes.GET_STORE_ROLES_REQUEST,
        StoreActionTypes.GET_STORE_ROLES_SUCCESS,
        StoreActionTypes.GET_STORE_ROLES_FAILURE,
      ],
    },
  };
}

export function fetchStoreSubordinates(storeGuid, ownershipGuid) {
  return {
    [CALL_API]: {
      endpoint: STORE_SUBORDINATES_URL.replace(
        "{storeGuid}",
        storeGuid
      ).replace("{ownershipGuid}", ownershipGuid),
      authenticated: true,
      types: [
        StoreActionTypes.GET_STORE_SUBORDINATES_REQUEST,
        StoreActionTypes.GET_STORE_SUBORDINATES_SUCCESS,
        StoreActionTypes.GET_STORE_SUBORDINATES_FAILURE,
      ],
    },
  };
}

export function addStoreOwner(owner, storeGuid) {
  return {
    [CALL_API]: {
      endpoint: ADD_STORE_OWNER_URL.replace("{storeGuid}", storeGuid),
      authenticated: true,
      types: [
        StoreActionTypes.ADD_STORE_OWNER_REQUEST,
        StoreActionTypes.ADD_STORE_OWNER_SUCCESS,
        StoreActionTypes.ADD_STORE_OWNER_FAILURE,
      ],
      config: {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(owner),
      },
    },
  };
}

export function addStoreManager(manager, storeGuid) {
  return {
    [CALL_API]: {
      endpoint: ADD_STORE_MANAGER_URL.replace("{storeGuid}", storeGuid),
      authenticated: true,
      types: [
        StoreActionTypes.ADD_STORE_MANAGER_REQUEST,
        StoreActionTypes.ADD_STORE_MANAGER_SUCCESS,
        StoreActionTypes.ADD_STORE_MANAGER_FAILURE,
      ],
      config: {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(manager),
      },
    },
  };
}

export function removeStoreManager(ownershipGuid, managmentToDeleteGuid) {
  return {
    [CALL_API]: {
      endpoint: REMOVE_SUBORDINATE_URL.replace("{ownershipGuid}", ownershipGuid)
        .replace("{roleToDeleteGuid}", managmentToDeleteGuid)
        .replace("{roleType}", "management"),
      authenticated: true,
      types: [
        StoreActionTypes.REMOVE_STORE_MANAGER_REQUEST,
        StoreActionTypes.REMOVE_STORE_MANAGER_SUCCESS,
        StoreActionTypes.REMOVE_STORE_MANAGER_FAILURE,
      ],
      config: {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
      },
    },
  };
}

export function removeStoreOwner(ownershipGuid, ownerdhipToDeleteGuid) {
  return {
    [CALL_API]: {
      endpoint: REMOVE_SUBORDINATE_URL.replace("{ownershipGuid}", ownershipGuid)
        .replace("{roleToDeleteGuid}", ownerdhipToDeleteGuid)
        .replace("{roleType}", "ownership"),
      authenticated: true,
      types: [
        StoreActionTypes.REMOVE_STORE_OWNER_REQUEST,
        StoreActionTypes.REMOVE_STORE_OWNER_SUCCESS,
        StoreActionTypes.REMOVE_STORE_OWNER_FAILURE,
      ],
      config: {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
      },
    },
  };
}

export function addStoreRootPolicy(storeGuid, rootPolicy) {
  return {
    [CALL_API]: {
      endpoint: STORE_POLICIES_URL.replace("{storeGuid}", storeGuid),
      authenticated: true,
      types: [
        StoreActionTypes.ADD_STORE_POLICY_ROOT_REQUEST,
        StoreActionTypes.ADD_STORE_POLICY_ROOT_SUCCESS,
        StoreActionTypes.ADD_STORE_POLICY_ROOT_FAILURE,
      ],
      config: {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(rootPolicy),
      },
    },
  };
}

export function addStoreSubPolicy(storeGuid, fatherPolicyGuid, subPolicy) {
  return {
    [CALL_API]: {
      endpoint: STORE_SUB_POLICIES_URL.replace(
        "{storeGuid}",
        storeGuid
      ).replace("{policyGuid}", fatherPolicyGuid),
      authenticated: true,
      types: [
        StoreActionTypes.ADD_STORE_SUB_POLICY_REQUEST,
        StoreActionTypes.ADD_STORE_SUB_POLICY_SUCCESS,
        StoreActionTypes.ADD_STORE_SUB_POLICY_FAILURE,
      ],
      config: {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(subPolicy),
      },
    },
  };
}

export function fetchStorePolicy(storeGuid) {
  return {
    [CALL_API]: {
      endpoint: STORE_POLICIES_URL.replace("{storeGuid}", storeGuid),
      authenticated: true,
      types: [
        StoreActionTypes.GET_STORE_POLICY_REQUEST,
        StoreActionTypes.GET_STORE_POLICY_SUCCESS,
        StoreActionTypes.GET_STORE_POLICY_FAILURE,
      ],
    },
  };
}

export function removeStorePolicy(storeGuid, policyToDeleteGuid) {
  return {
    [CALL_API]: {
      endpoint: STORE_POLICY_URL.replace("{storeGuid}", storeGuid).replace(
        "{policyGuid}",
        policyToDeleteGuid
      ),
      authenticated: true,
      types: [
        StoreActionTypes.REMOVE_STORE_POLICY_REQUEST,
        StoreActionTypes.REMOVE_STORE_POLICY_SUCCESS,
        StoreActionTypes.REMOVE_STORE_POLICY_FAILURE,
      ],
      config: {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
      },
    },
  };
}

export function addStoreRootDiscount() {}
export function addStoreSubDiscount() {}
export function fetchStoreDiscounts() {}
export function removeStoreDiscount() {}
