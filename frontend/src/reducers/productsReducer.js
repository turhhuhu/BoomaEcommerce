import * as ProductsActionTypes from "../actions/types/productsActionsTypes";
export function productsView(
  state = {
    isFetching: false,
    products: [],
    authenticated: false,
  },
  action
) {
  switch (action.type) {
    case ProductsActionTypes.GET_ALL_PRODUCTS_REQUEST:
      return state;
    case ProductsActionTypes.GET_ALL_PRODUCTS_SUCCESS:
      console.log(action);
      return Object.assign({}, state, {
        products: action.payload.response,
      });
    case ProductsActionTypes.GET_ALL_PRODUCTS_FAILURE:
      console.error(
        `error occured while getting all products: ${action.error}`
      );
      return state;
    default:
      return state;
  }
}
