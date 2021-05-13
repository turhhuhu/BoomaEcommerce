import * as StoreActionTypes from "../actions/types/storeActionsTypes";
export function store(
  state = {
    isFetching: false,
    products: [],
    filteredProducts: [],
    storeInfo: {},
    storeOwners: [],
    storeManagers: [],
  },
  action
) {
  switch (action.type) {
    case StoreActionTypes.STORE_INFO_REQUEST:
      return Object.assign({}, state, { ...action.payload, error: undefined });
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
      return Object.assign({}, state, { ...action.payload, error: undefined });
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
      return Object.assign({}, state, { ...action.payload, error: undefined });
    case StoreActionTypes.ADD_PRODUCT_TO_STORE_SUCCESS:
      return Object.assign({}, state, {
        products: [...state.products, action.payload.response],
      });
    case StoreActionTypes.ADD_PRODUCT_TO_STORE_FAILURE:
      console.error(
        `error occured while adding product to a store: ${action.error}`
      );
      return Object.assign({}, state, {
        error: action.error,
        isFetching: action.payload.isFetching,
      });
    default:
      return state;
  }
}
