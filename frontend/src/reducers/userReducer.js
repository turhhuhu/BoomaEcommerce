import * as UserActionTypes from "../actions/types/userActionsTypes";
export function user(
  state = {
    isFetching: false,
    userInfo: {},
    userRoles: {},
    cart: {
      baskets: [],
    },
    webSocketConnection: undefined,
    notifications: [],
  },
  action
) {
  // TODO: might need to add functionality of editing purchase product inside basket.

  switch (action.type) {
    case UserActionTypes.USER_INFO_REQUEST:
      return Object.assign({}, state, action.payload);
    case UserActionTypes.USER_INFO_SUCCESS:
      //TODO: add notifications to state when the backend is ready
      const { notifications, ...userInfo } = action.payload.response;
      return Object.assign({}, state, {
        userInfo: userInfo,
        isFetching: action.payload.isFetching,
      });
    case UserActionTypes.USER_INFO_FAILURE:
      console.error(`error occured while getting user info: ${action.error}`);
      return state;
    case UserActionTypes.USER_CART_REQUEST:
      return Object.assign({}, state, action.payload);
    case UserActionTypes.USER_CART_SUCCESS:
      return Object.assign({}, state, {
        cart: action.payload.response,
        isFetching: action.payload.isFetching,
      });
    case UserActionTypes.USER_CART_FAILURE:
      console.error(`error occured while getting user's cart: ${action.error}`);
      return Object.assign({}, state, action.payload);
    case UserActionTypes.ADD_PRODUCT_WITH_BASKET_REQUEST:
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case UserActionTypes.ADD_PRODUCT_WITH_BASKET_SUCCESS:
      return Object.assign({}, state, {
        cart: {
          baskets: [...state.cart.baskets, action.payload.response],
        },
        isFetching: action.payload.isFetching,
      });
    case UserActionTypes.ADD_PRODUCT_WITH_BASKET_FAILURE:
      console.error(
        `error occured while adding basket with product: ${action.error}`
      );
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case UserActionTypes.ADD_PRODUCT_TO_BASKET_REQUEST:
      return Object.assign({}, state, action.payload);
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
            [basketToAddToIndex]: newBasket,
          }),
        },
      });
    }
    case UserActionTypes.ADD_PRODUCT_TO_BASKET_FAILURE:
      console.error(
        `error occured while adding product to basket: ${action.error}`
      );
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case UserActionTypes.REMOVE_PURCHASE_PRODUCT_FROM_BASKET_REQUEST:
      return Object.assign({}, state, action.payload);
    case UserActionTypes.REMOVE_PURCHASE_PRODUCT_FROM_BASKET_SUCCESS: {
      const basketToAddToIndex = state.cart.baskets.findIndex(
        (basket) => basket.guid === action.extraPayload.basketGuid
      );
      const basketToRemoveFrom = state.cart.baskets[basketToAddToIndex];
      const purchaseProductToRemoveIndex =
        basketToRemoveFrom.purchaseProducts.findIndex(
          (purchaseProduct) =>
            purchaseProduct.guid === action.extraPayload.purchaseProductGuid
        );
      const newBasket = {
        ...basketToRemoveFrom,
        purchaseProducts: [
          ...basketToRemoveFrom.purchaseProducts.slice(
            0,
            purchaseProductToRemoveIndex
          ),
          ...basketToRemoveFrom.purchaseProducts.slice(
            purchaseProductToRemoveIndex + 1
          ),
        ],
      };
      return Object.assign({}, state, {
        cart: {
          baskets: Object.assign([], state.cart.baskets, {
            [basketToAddToIndex]: newBasket,
          }),
        },
      });
    }
    case UserActionTypes.REMOVE_PURCHASE_PRODUCT_FROM_BASKET_FAILURE:
      console.error(
        `error occured while removing product from basket: ${action.error}`
      );
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });

    case UserActionTypes.USER_ROLES_REQUEST:
      return Object.assign({}, state, action.payload);
    case UserActionTypes.USER_ROLES_SUCCESS:
      return Object.assign({}, state, {
        userRoles: action.payload.response,
        isFetching: action.payload.isFetching,
      });
    case UserActionTypes.USER_ROLES_FAILURE:
      console.error(`error occured while getting user roles: ${action.error}`);
      return state;
    case UserActionTypes.ADD_STORE_REQUEST:
      return Object.assign({}, state, action.payload);
    case UserActionTypes.ADD_STORE_SUCCESS:
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case UserActionTypes.ADD_STORE_FAILURE:
      console.error(`error occured while adding store: ${action.error}`);
      return state;
    case UserActionTypes.USER_STORE_ROLE_REQUEST:
      return Object.assign({}, state, action.payload);
    case UserActionTypes.USER_STORE_ROLE_SUCCESS:
      return Object.assign({}, state, {
        storeRole: action.payload.response,
        isFetching: action.payload.isFetching,
      });
    case UserActionTypes.USER_STORE_ROLE_FAILURE:
      console.error(
        `error occured while getting user store role: ${action.error}`
      );
      return state;
    case UserActionTypes.START_WEB_SOCKET_CONNECTION:
      return Object.assign({}, state, {
        webSocketConnection: action.payload.webSocketConnection,
      });
    case UserActionTypes.CLOSE_WEB_SOCKET_CONNECTION:
      return Object.assign({}, state, {
        webSocketConnection: undefined,
      });
    case UserActionTypes.RECIEVE_REGULAR_NOTIFICATION:
      return Object.assign({}, state, {
        notifications: [...state.notifications, action.payload.notification],
      });
    case UserActionTypes.SEE_NOTIFICATION: {
      const seenNotificationIndex = action.payload.seenNotificationIndex;
      const seenNotification = action.payload.seenNotification;
      console.log(seenNotificationIndex);
      console.log(seenNotification);
      const newNotifcations = [
        ...state.notifications.slice(0, seenNotificationIndex),
        seenNotification,
        ...state.notifications.slice(seenNotificationIndex + 1),
      ];
      return Object.assign({}, state, {
        notifications: newNotifcations,
      });
    }

    default:
      return state;
  }
}
