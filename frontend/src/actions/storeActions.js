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
  STORE_DISCOUNTS_URL,
  STORE_DISCOUNT_URL,
  STORE_SUB_DISCOUNTS_URL,
  STORE_DISCOUNT_POLICIES_URL,
  STORE_DISCOUNT_SUB_POLICIES_URL,
  STORE_PURCHASE_HISTORY_URL,
  MANAGER_PERMISSIONS_URL,
  STORE_PRODUCT_OFFERS_URL,
  STORE_PRODUCT_COUNTER_OFFER_URL,
  APPROVE_PRODUCT_OFFER_URL,
  DECLINE_PRODUCT_OFFER_URL,
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

export function addStoreRootDiscount(storeGuid, rootDiscount) {
  return {
    [CALL_API]: {
      endpoint: STORE_DISCOUNTS_URL.replace("{storeGuid}", storeGuid),
      authenticated: true,
      types: [
        StoreActionTypes.ADD_STORE_DISCOUNT_ROOT_REQUEST,
        StoreActionTypes.ADD_STORE_DISCOUNT_ROOT_SUCCESS,
        StoreActionTypes.ADD_STORE_DISCOUNT_ROOT_FAILURE,
      ],
      config: {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(rootDiscount),
      },
    },
  };
}

export function addStoreSubDiscount(storeGuid, fatherPolicyGuid, subDiscount) {
  return {
    [CALL_API]: {
      endpoint: STORE_SUB_DISCOUNTS_URL.replace(
        "{storeGuid}",
        storeGuid
      ).replace("{discountGuid}", fatherPolicyGuid),
      authenticated: true,
      types: [
        StoreActionTypes.ADD_STORE_SUB_DISCOUNT_REQUEST,
        StoreActionTypes.ADD_STORE_SUB_DISCOUNT_SUCCESS,
        StoreActionTypes.ADD_STORE_SUB_DISCOUNT_FAILURE,
      ],
      config: {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(subDiscount),
      },
    },
  };
}

export function fetchStoreDiscounts(storeGuid) {
  return {
    [CALL_API]: {
      endpoint: STORE_DISCOUNTS_URL.replace("{storeGuid}", storeGuid),
      authenticated: true,
      types: [
        StoreActionTypes.GET_STORE_DISCOUNT_REQUEST,
        StoreActionTypes.GET_STORE_DISCOUNT_SUCCESS,
        StoreActionTypes.GET_STORE_DISCOUNT_FAILURE,
      ],
    },
  };
}
export function removeStoreDiscount(storeGuid, discountToDeleteGuid) {
  return {
    [CALL_API]: {
      endpoint: STORE_DISCOUNT_URL.replace("{storeGuid}", storeGuid).replace(
        "{discountGuid}",
        discountToDeleteGuid
      ),
      authenticated: true,
      types: [
        StoreActionTypes.REMOVE_STORE_DISCOUNT_REQUEST,
        StoreActionTypes.REMOVE_STORE_DISCOUNT_SUCCESS,
        StoreActionTypes.REMOVE_STORE_DISCOUNT_FAILURE,
      ],
      config: {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
      },
    },
  };
}

export function addStoreDicountRootPolicy(discountGuid) {
  return function (storeGuid, rootPolicy) {
    console.log(storeGuid, discountGuid);
    return {
      [CALL_API]: {
        endpoint: STORE_DISCOUNT_POLICIES_URL.replace(
          "{storeGuid}",
          storeGuid
        ).replace("{discountGuid}", discountGuid),
        authenticated: true,
        types: [
          StoreActionTypes.ADD_STORE_DISCOUNT_POLICY_ROOT_REQUEST,
          StoreActionTypes.ADD_STORE_DISCOUNT_POLICY_ROOT_SUCCESS,
          StoreActionTypes.ADD_STORE_DISCOUNT_POLICY_ROOT_FAILURE,
        ],
        config: {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(rootPolicy),
        },
      },
    };
  };
}
export function addStoreDicountSubPolicy(discountGuid) {
  return function (storeGuid, fatherPolicyGuid, subPolicy) {
    return {
      [CALL_API]: {
        endpoint: STORE_DISCOUNT_SUB_POLICIES_URL.replace(
          "{storeGuid}",
          storeGuid
        )
          .replace("{discountGuid}", discountGuid)
          .replace("{policyGuid}", fatherPolicyGuid),
        authenticated: true,
        types: [
          StoreActionTypes.ADD_STORE_DISCOUNT_SUB_POLICY_REQUEST,
          StoreActionTypes.ADD_STORE_DISCOUNT_SUB_POLICY_SUCCESS,
          StoreActionTypes.ADD_STORE_DISCOUNT_SUB_POLICY_FAILURE,
        ],
        config: {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(subPolicy),
        },
      },
    };
  };
}

export function fetchStoreDiscountPolicy(discountGuid) {
  return function (storeGuid) {
    return {
      [CALL_API]: {
        endpoint: STORE_DISCOUNT_POLICIES_URL.replace(
          "{storeGuid}",
          storeGuid
        ).replace("{discountGuid}", discountGuid),
        authenticated: true,
        types: [
          StoreActionTypes.GET_STORE_DISCOUNT_POLICY_REQUEST,
          StoreActionTypes.GET_STORE_DISCOUNT_POLICY_SUCCESS,
          StoreActionTypes.GET_STORE_DISCOUNT_POLICY_FAILURE,
        ],
      },
    };
  };
}

export function removeStoreDiscountPolicy(discountGuid) {
  return function (storeGuid, policyToDeleteGuid) {
    return {
      [CALL_API]: {
        endpoint: STORE_DISCOUNT_POLICIES_URL.replace("{storeGuid}", storeGuid)
          .replace("{discountGuid}", discountGuid)
          .replace("{policyGuid}", policyToDeleteGuid),
        authenticated: true,
        types: [
          StoreActionTypes.REMOVE_STORE_DISCOUNT_POLICY_REQUEST,
          StoreActionTypes.REMOVE_STORE_DISCOUNT_POLICY_SUCCESS,
          StoreActionTypes.REMOVE_STORE_DISCOUNT_POLICY_FAILURE,
        ],
        config: {
          method: "DELETE",
          headers: { "Content-Type": "application/json" },
        },
      },
    };
  };
}

export function fetchStorePurchaseHistory(storeGuid) {
  return {
    [CALL_API]: {
      endpoint: STORE_PURCHASE_HISTORY_URL.replace("{storeGuid}", storeGuid),
      authenticated: true,
      types: [
        StoreActionTypes.GET_STORE_PURCHASE_HISTORY_REQUEST,
        StoreActionTypes.GET_STORE_PURCHASE_HISTORY_SUCCESS,
        StoreActionTypes.GET_STORE_PURCHASE_HISTORY_FAILURE,
      ],
    },
  };
}

export function editManagerPermissions(permissions, managementGuid) {
  return {
    [CALL_API]: {
      endpoint: MANAGER_PERMISSIONS_URL.replace(
        "{managementGuid}",
        managementGuid
      ),
      authenticated: true,
      types: [
        StoreActionTypes.EDIT_STORE_MANAGER_PERMISSIONS_REQUEST,
        StoreActionTypes.EDIT_STORE_MANAGER_PERMISSIONS_SUCCESS,
        StoreActionTypes.EDIT_STORE_MANAGER_PERMISSIONS_FAILURE,
      ],
      config: {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(permissions),
      },
    },
  };
}

export function fetchStoreProductOffers(ownershipGuid) {
  return {
    [CALL_API]: {
      endpoint: STORE_PRODUCT_OFFERS_URL.replace(
        "{ownershipGuid}",
        ownershipGuid
      ),
      authenticated: true,
      types: [
        StoreActionTypes.GET_STORE_PRODUCT_OFFERS_REQUEST,
        StoreActionTypes.GET_STORE_PRODUCT_OFFERS_SUCCESS,
        StoreActionTypes.GET_STORE_PRODUCT_OFFERS_FAILURE,
      ],
    },
  };
}

export function counterOfferProductPrice(ownershipGuid, offerGuid, price) {
  return {
    [CALL_API]: {
      endpoint: STORE_PRODUCT_COUNTER_OFFER_URL.replace(
        "{ownershipGuid}",
        ownershipGuid
      )
        .replace("{offerGuid}", offerGuid)
        .replace("{price}", price),
      authenticated: true,
      types: [
        StoreActionTypes.COUNTER_OFFER_STORE_PRODUCT_PRICE_REQUEST,
        StoreActionTypes.COUNTER_OFFER_STORE_PRODUCT_PRICE_SUCCESS,
        StoreActionTypes.COUNTER_OFFER_STORE_PRODUCT_PRICE_FAILURE,
      ],
      config: {
        method: "POST",
        headers: { "Content-Type": "application/json" },
      },
    },
  };
}

export function approveProductOffer(ownershipGuid, offerGuid) {
  return {
    [CALL_API]: {
      endpoint: APPROVE_PRODUCT_OFFER_URL.replace(
        "{ownershipGuid}",
        ownershipGuid
      ).replace("{offerGuid}", offerGuid),
      authenticated: true,
      types: [
        StoreActionTypes.APPROVE_STORE_PRODUCT_OFFER_REQUEST,
        StoreActionTypes.APPROVE_STORE_PRODUCT_OFFER_SUCCESS,
        StoreActionTypes.APPROVE_STORE_PRODUCT_OFFER_FAILURE,
      ],
      config: {
        method: "POST",
        headers: { "Content-Type": "application/json" },
      },
    },
  };
}

export function declineProductOffer(ownershipGuid, offerGuid) {
  return {
    [CALL_API]: {
      endpoint: DECLINE_PRODUCT_OFFER_URL.replace(
        "{ownershipGuid}",
        ownershipGuid
      ).replace("{offerGuid}", offerGuid),
      authenticated: true,
      types: [
        StoreActionTypes.DECLINE_STORE_PRODUCT_OFFER_REQUEST,
        StoreActionTypes.DECLINE_STORE_PRODUCT_OFFER_SUCCESS,
        StoreActionTypes.DECLINE_STORE_PRODUCT_OFFER_FAILURE,
      ],
      config: {
        method: "POST",
        headers: { "Content-Type": "application/json" },
      },
    },
  };
}
