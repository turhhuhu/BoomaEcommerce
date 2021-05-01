import * as UserActionTypes from "../actions/types/userActionsTypes";
export function user(
  state = {
    isFetching: false,
    userInfo: {},
    cart: {
      baskets: [],
      guid: undefined,
    },
  },
  action
) {
  // TODO: might need to add functionality of editing purchase product inside basket.

  switch (action.type) {
    case UserActionTypes.USER_INFO_REQUEST:
      return state;
    case UserActionTypes.USER_INFO_SUCCESS:
      console.log(action);
      return Object.assign({}, state, {
        userInfo: action.payload.response,
      });
    case UserActionTypes.USER_INFO_FAILURE:
      console.error(`error occured while getting user info: ${action.error}`);
      return state;
    case UserActionTypes.USER_CART_REQUEST:
      return state;
    case UserActionTypes.USER_CART_SUCCESS:
      return Object.assign({}, state, {
        cart: action.payload.response,
      });
    case UserActionTypes.USER_CART_FAILURE:
      console.error(`error occured while getting user info: ${action.error}`);
      return state;
    case UserActionTypes.ADD_PRODUCT_WITH_BASKET_REQUEST:
      return state;
    case UserActionTypes.ADD_PRODUCT_WITH_BASKET_SUCCESS:
      return Object.assign({}, state, {
        cart: {
          baskets: [...state.cart.baskets, action.payload.response],
        },
      });
    case UserActionTypes.ADD_PRODUCT_WITH_BASKET_FAILURE:
      console.error(`error occured while getting user info: ${action.error}`);
      return state;
    case UserActionTypes.ADD_PRODUCT_TO_BASKET_REQUEST:
      return state;
    case UserActionTypes.ADD_PRODUCT_TO_BASKET_SUCCESS: {
      const basketToAddToIndex = state.cart.baskets.findIndex(
        (basket) => basket.guid === action.extraPayload
      );
      const basketToAddTo = state.cart.baskets[basketToAddToIndex];
      const newBasket = {
        ...basketToAddTo,
        purchaseProducts: [
          ...basketToAddTo.purchaseProducts,
          action.payload.response,
        ],
      };
      return Object.assign({}, state, {
        cart: {
          baskets: Object.assign([], state.cart.baskets, {
            basketToAddToIndex: newBasket,
          }),
        },
      });
    }
    case UserActionTypes.ADD_PRODUCT_TO_BASKET_FAILURE:
      console.error(`error occured while getting user info: ${action.error}`);
      return state;
    default:
      return state;
  }
}
