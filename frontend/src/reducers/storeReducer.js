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
      return Object.assign({}, state, action.payload);
    case StoreActionTypes.STORE_INFO_SUCCESS:
      return Object.assign({}, state, {
        storeInfo: action.payload.response,
        isFetching: action.payload.isFetching,
      });
    case StoreActionTypes.STORE_INFO_FAILURE:
      console.error(`error occured while getting store info: ${action.error}`);
      return state;
    case StoreActionTypes.GET_ALL_STORE_PRODUCTS_REQUEST:
      return state;
    case StoreActionTypes.GET_ALL_STORE_PRODUCTS_SUCCESS:
      return Object.assign({}, state, {
        products: action.payload.response,
        filteredProducts: action.payload.response,
      });
    case StoreActionTypes.GET_ALL_STORE_PRODUCTS_FAILURE:
      console.error(
        `error occured while getting all store products: ${action.error}`
      );
      return state;
    case StoreActionTypes.FILTER_STORE_PRODUCTS:
      return Object.assign({}, state, {
        filteredProducts: action.payload.filteredProducts,
      });
    default:
      return state;
  }
}
