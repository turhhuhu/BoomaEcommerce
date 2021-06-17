import * as StoreActionTypes from "../actions/types/storeActionsTypes";
export function store(
  state = {
    isFetching: false,
    products: [],
    filteredProducts: [],
    storeInfo: {},
    storeRoles: {},
    subordinates: [],
    storePolicy: {},
    storeDiscount: {},
    storeDiscountPolicy: {},
    purchaseHistory: [],
    productOffers: [],
  },
  action
) {
  switch (action.type) {
    case StoreActionTypes.STORE_INFO_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
      });
    case StoreActionTypes.STORE_INFO_SUCCESS:
      return Object.assign({}, state, {
        storeInfo: action.payload.response,
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.STORE_INFO_FAILURE:
      console.error(`error occured while getting store info: ${action.error}`);
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.GET_ALL_STORE_PRODUCTS_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
      });
    case StoreActionTypes.GET_ALL_STORE_PRODUCTS_SUCCESS:
      return Object.assign({}, state, {
        products: action.payload.response,
        filteredProducts: action.payload.response,
      });
    case StoreActionTypes.GET_ALL_STORE_PRODUCTS_FAILURE:
      console.error(
        `error occured while getting all store products: ${action.error}`
      );
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.FILTER_STORE_PRODUCTS:
      return Object.assign({}, state, {
        filteredProducts: action.payload.filteredProducts,
        error: undefined,
      });
    case StoreActionTypes.ADD_PRODUCT_TO_STORE_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.ADD_PRODUCT_TO_STORE_SUCCESS: {
      const newProducts = [...state.products, action.payload.response];
      return Object.assign({}, state, {
        products: newProducts,
        filteredProducts: newProducts,
        isFetching: action.payload.isFetching,
      });
    }
    case StoreActionTypes.ADD_PRODUCT_TO_STORE_FAILURE:
      console.error(
        `error occured while adding product to a store: ${action.error}`
      );
      return Object.assign({}, state, {
        error: action.error,
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.REMOVE_PRODUCT_FROM_STORE_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
      });
    case StoreActionTypes.REMOVE_PRODUCT_FROM_STORE_SUCCESS: {
      const productToRemoveIndexInProducts = state.products.findIndex(
        (product) => product.guid === action.extraPayload.productGuid
      );
      const productToRemoveIndexInFilteredProducts =
        state.filteredProducts.findIndex(
          (product) => product.guid === action.extraPayload.productGuid
        );
      const newProducts = [
        ...state.products.slice(0, productToRemoveIndexInProducts),
        ...state.products.slice(productToRemoveIndexInProducts + 1),
      ];
      const newFilteredProducts = [
        ...state.filteredProducts.slice(
          0,
          productToRemoveIndexInFilteredProducts
        ),
        ...state.filteredProducts.slice(
          productToRemoveIndexInFilteredProducts + 1
        ),
      ];
      return Object.assign({}, state, {
        products: newProducts,
        filteredProducts:
          productToRemoveIndexInFilteredProducts !== -1
            ? newFilteredProducts
            : state.filteredProducts,
      });
    }
    case StoreActionTypes.REMOVE_PRODUCT_FROM_STORE_FAILURE:
      console.error(
        `error occured while removing product from a store: ${action.error}`
      );
      return Object.assign({}, state, {
        error: action.error,
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.EDIT_PRODUCT_FROM_STORE_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.EDIT_PRODUCT_FROM_STORE_SUCCESS:
      const productToEditIndexInProducts = state.products.findIndex(
        (product) => product.guid === action.extraPayload.productGuid
      );
      const productToEditIndexInFilteredProducts = state.products.findIndex(
        (product) => product.guid === action.extraPayload.productGuid
      );
      const productToEdit = state.products[productToEditIndexInProducts];
      const editedName = action.extraPayload.editedProduct.name;
      const editedCategory = action.extraPayload.editedProduct.category;
      const editedAmount = action.extraPayload.editedProduct.amount;
      const editedPrice = action.extraPayload.editedProduct.price;
      const editedProduct = {
        ...productToEdit,
        name: editedName ? editedName : productToEdit.name,
        category: editedCategory ? editedCategory : productToEdit.category,
        amount: editedAmount ? editedAmount : productToEdit.amount,
        editedPrice: editedPrice ? editedPrice : productToEdit.price,
      };
      const newProducts = Object.assign([], state.products, {
        [productToEditIndexInProducts]: editedProduct,
      });
      const newFilteredProducts = Object.assign([], state.filteredProducts, {
        [productToEditIndexInFilteredProducts]: editedProduct,
      });
      return Object.assign({}, state, {
        products: newProducts,
        filteredProducts:
          productToEditIndexInFilteredProducts !== -1
            ? newFilteredProducts
            : state.filteredProducts,
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.EDIT_PRODUCT_FROM_STORE_FAILURE:
      console.error(
        `error occured while editing store product: ${action.error}`
      );
      return Object.assign({}, state, {
        error: action.error,
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.GET_STORE_ROLES_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
      });
    case StoreActionTypes.GET_STORE_ROLES_SUCCESS:
      return Object.assign({}, state, {
        storeRoles: action.payload.response,
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.GET_STORE_ROLES_FAILURE:
      console.error(`error occured while getting store roles: ${action.error}`);
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.GET_STORE_SUBORDINATES_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
      });
    case StoreActionTypes.GET_STORE_SUBORDINATES_SUCCESS:
      return Object.assign({}, state, {
        subordinates: action.payload.response,
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.GET_STORE_SUBORDINATES_FAILURE:
      console.error(
        `error occured while getting store subordinates: ${action.error}`
      );
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });

    case StoreActionTypes.ADD_STORE_OWNER_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
      });
    case StoreActionTypes.ADD_STORE_OWNER_SUCCESS: {
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    }
    case StoreActionTypes.ADD_STORE_OWNER_FAILURE:
      console.error(`error occured while adding store owner: ${action.error}`);
      return Object.assign({}, state, {
        error: action.error,
        isFetching: action.payload.isFetching,
      });

    case StoreActionTypes.ADD_STORE_MANAGER_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
      });
    case StoreActionTypes.ADD_STORE_MANAGER_SUCCESS: {
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    }
    case StoreActionTypes.ADD_STORE_MANAGER_FAILURE: {
      console.error(`error occured while adding store owner: ${action.error}`);
      return Object.assign({}, state, {
        error: action.error,
        isFetching: action.payload.isFetching,
      });
    }
    case StoreActionTypes.REMOVE_STORE_MANAGER_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
      });
    case StoreActionTypes.REMOVE_STORE_MANAGER_SUCCESS:
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.REMOVE_STORE_MANAGER_FAILURE: {
      console.error(
        `error occured while remvoving store manager: ${action.error}`
      );
      return Object.assign({}, state, {
        error: action.error,
        isFetching: action.payload.isFetching,
      });
    }
    case StoreActionTypes.REMOVE_STORE_OWNER_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
      });
    case StoreActionTypes.REMOVE_STORE_OWNER_SUCCESS:
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.REMOVE_STORE_OWNER_FAILURE: {
      console.error(
        `error occured while remvoving store owner: ${action.error}`
      );
      return Object.assign({}, state, {
        error: action.error,
        isFetching: action.payload.isFetching,
      });
    }
    case StoreActionTypes.GET_STORE_POLICY_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        storePolicy: {},
        error: undefined,
      });
    case StoreActionTypes.GET_STORE_POLICY_SUCCESS:
      return Object.assign({}, state, {
        storePolicy: action.payload.response,
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.GET_STORE_POLICY_FAILURE:
      console.error(
        `error occured while getting store policy: ${action.error}`
      );
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.REMOVE_STORE_POLICY_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
      });
    case StoreActionTypes.REMOVE_STORE_POLICY_SUCCESS: {
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    }
    case StoreActionTypes.REMOVE_STORE_POLICY_FAILURE: {
      console.error(
        `error occured while remvoving store sub-policy: ${action.error}`
      );
      return Object.assign({}, state, {
        error: action.error,
        isFetching: action.payload.isFetching,
      });
    }
    case StoreActionTypes.ADD_STORE_POLICY_ROOT_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
      });
    case StoreActionTypes.ADD_STORE_POLICY_ROOT_SUCCESS:
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.ADD_STORE_POLICY_ROOT_FAILURE: {
      console.error(`error occured while adding store policy: ${action.error}`);
      return Object.assign({}, state, {
        error: action.error,
        isFetching: action.payload.isFetching,
      });
    }
    case StoreActionTypes.ADD_STORE_SUB_POLICY_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
      });
    case StoreActionTypes.ADD_STORE_SUB_POLICY_SUCCESS:
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.ADD_STORE_SUB_POLICY_FAILURE: {
      console.error(`error occured while adding store policy: ${action.error}`);
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    }
    case StoreActionTypes.GET_STORE_DISCOUNT_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        storeDiscount: {},
        error: undefined,
      });
    case StoreActionTypes.GET_STORE_DISCOUNT_SUCCESS:
      return Object.assign({}, state, {
        storeDiscount: action.payload.response,
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.GET_STORE_DISCOUNT_FAILURE:
      console.error(
        `error occured while getting store discount: ${action.error}`
      );
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });

    case StoreActionTypes.GET_STORE_DISCOUNT_POLICY_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        storeDiscountPolicy: {},
        error: undefined,
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.GET_STORE_DISCOUNT_POLICY_SUCCESS:
      return Object.assign({}, state, {
        storeDiscountPolicy: action.payload.response,
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.GET_STORE_DISCOUNT_POLICY_FAILURE:
      console.error(
        `error occured while getting store discount policy: ${action.error}`
      );
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.GET_STORE_PURCHASE_HISTORY_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.GET_STORE_PURCHASE_HISTORY_SUCCESS:
      return Object.assign({}, state, {
        purchaseHistory: action.payload.response,
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.GET_STORE_PURCHASE_HISTORY_FAILURE:
      console.error(
        `error occured while fetching purchase history: ${action.error}`
      );
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.GET_STORE_PRODUCT_OFFERS_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.GET_STORE_PRODUCT_OFFERS_SUCCESS:
      return Object.assign({}, state, {
        productOffers: action.payload.response,
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.GET_STORE_PRODUCT_OFFERS_FAILURE:
      console.error(
        `error occured while fetching product offers: ${action.error}`
      );
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });

    case StoreActionTypes.ADD_STORE_DISCOUNT_ROOT_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
        isFetching: action.payload.isFetching,
      });

    case StoreActionTypes.ADD_STORE_DISCOUNT_ROOT_SUCCESS:
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.ADD_STORE_DISCOUNT_ROOT_FAILURE:
      console.error(`error occured: ${action.error}`);
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
        error: action.error,
      });
    case StoreActionTypes.ADD_STORE_SUB_DISCOUNT_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
        isFetching: action.payload.isFetching,
      });

    case StoreActionTypes.ADD_STORE_SUB_DISCOUNT_SUCCESS:
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.ADD_STORE_SUB_DISCOUNT_FAILURE:
      console.error(`error occured: ${action.error}`);
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
        error: action.error,
      });
    case StoreActionTypes.ADD_STORE_DISCOUNT_POLICY_ROOT_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
        isFetching: action.payload.isFetching,
      });

    case StoreActionTypes.ADD_STORE_DISCOUNT_POLICY_ROOT_SUCCESS:
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.ADD_STORE_DISCOUNT_POLICY_ROOT_FAILURE:
      console.error(`error occured: ${action.error}`);
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
        error: action.error,
      });
    case StoreActionTypes.ADD_STORE_DISCOUNT_SUB_POLICY_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
        isFetching: action.payload.isFetching,
      });

    case StoreActionTypes.ADD_STORE_DISCOUNT_SUB_POLICY_SUCCESS:
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.ADD_STORE_DISCOUNT_SUB_POLICY_FAILURE:
      console.error(`error occured: ${action.error}`);
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
        error: action.error,
      });
    case StoreActionTypes.COUNTER_OFFER_STORE_PRODUCT_PRICE_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
        isFetching: action.payload.isFetching,
      });

    case StoreActionTypes.COUNTER_OFFER_STORE_PRODUCT_PRICE_SUCCESS:
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.COUNTER_OFFER_STORE_PRODUCT_PRICE_FAILURE:
      console.error(`error occured: ${action.error}`);
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
        error: action.error,
      });

    default:
      return state;
  }
}
