import * as ProductsActionTypes from "../actions/types/productsActionsTypes";
export function products(
  state = {
    isFetching: false,
    products: [],
    filteredProducts: [],
  },
  action
) {
  switch (action.type) {
    case ProductsActionTypes.GET_ALL_PRODUCTS_REQUEST:
      return state;
    case ProductsActionTypes.GET_ALL_PRODUCTS_SUCCESS:
      return Object.assign({}, state, {
        products: action.payload.response,
        filteredProducts: action.payload.response,
      });
    case ProductsActionTypes.GET_ALL_PRODUCTS_FAILURE:
      console.error(
        `error occured while getting all products: ${action.error}`
      );
      return state;
    case ProductsActionTypes.FILTER_PRODUCTS:
      return Object.assign({}, state, {
        filteredProducts: action.payload.filteredProducts,
      });
    default:
      return state;
  }
}
