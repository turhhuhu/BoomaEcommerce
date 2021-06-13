import * as UserActionTypes from "../actions/types/userActionsTypes";
export function user(
  state = {
    isFetching: false,
    error: undefined,
    userInfo: {},
    userRoles: {},
    cart: {
      baskets: [],
    },
    webSocketConnection: undefined,
    notifications: [],
    paymentInfo: {},
    deliveryInfo: {},
  },
  action
) {
  // TODO: might need to add functionality of editing purchase product inside basket.

  switch (action.type) {
    case UserActionTypes.USER_INFO_REQUEST:
      return Object.assign({}, state, action.payload);
    case UserActionTypes.USER_INFO_SUCCESS:
      const { notifications, ...userInfo } = action.payload.response;
      return Object.assign({}, state, {
        notifications: notifications,
        userInfo: userInfo,
        isFetching: action.payload.isFetching,
      });
    case UserActionTypes.USER_INFO_FAILURE:
      console.error(`error occured while getting user info: ${action.error}`);
      return state;
    case UserActionTypes.USER_CART_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
      });
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
    case UserActionTypes.ADD_PRODUCT_WITH_BASKET_AS_GUEST:
      console.log(action.payload.basket);
      return Object.assign({}, state, {
        cart: {
          baskets: [...state.cart.baskets, action.payload.basket],
        },
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
    case UserActionTypes.ADD_PRODUCT_TO_BASKET_AS_GUEST: {
      const basketToAddToIndex = state.cart.baskets.findIndex(
        (basket) => basket.guid === action.payload.basketGuid
      );
      const basketToAddTo = state.cart.baskets[basketToAddToIndex];
      const newBasket = {
        ...basketToAddTo,
        purchaseProducts: [
          ...basketToAddTo.purchaseProducts,
          action.payload.purchaseProduct.purchaseProduct,
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
    case UserActionTypes.REMOVE_PRODUCT_FROM_BASKET_REQUEST:
      return Object.assign({}, state, action.payload);
    case UserActionTypes.REMOVE_PRODUCT_FROM_BASKET_SUCCESS: {
      const basketToRemoveFromIndex = state.cart.baskets.findIndex(
        (basket) => basket.guid === action.extraPayload.basketGuid
      );
      const basketToRemoveFrom = state.cart.baskets[basketToRemoveFromIndex];
      const purchaseProductToRemoveIndex =
        basketToRemoveFrom.purchaseProducts.findIndex(
          (purchaseProduct) =>
            purchaseProduct.guid === action.extraPayload.purchaseProductGuid
        );
      if (
        state.cart.baskets[basketToRemoveFromIndex].purchaseProducts.length ===
        1
      ) {
        return Object.assign({}, state, {
          cart: {
            baskets: [
              ...state.cart.baskets.slice(0, basketToRemoveFromIndex),
              ...state.cart.baskets.slice(basketToRemoveFromIndex + 1),
            ],
          },
        });
      }
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
            [basketToRemoveFromIndex]: newBasket,
          }),
        },
      });
    }

    case UserActionTypes.REMOVE_PRODUCT_FROM_BASKET_FAILURE:
      console.error(
        `error occured while removing product from basket: ${action.error}`
      );
      return Object.assign({}, state, {
        isFetching: action.payload.isFetching,
      });
    case UserActionTypes.REMOVE_PRODUCT_FROM_BASKET_AS_GUEST:
      const basketToRemoveFromIndex = state.cart.baskets.findIndex(
        (basket) => basket.guid === action.payload.basketGuid
      );
      const basketToRemoveFrom = state.cart.baskets[basketToRemoveFromIndex];
      const purchaseProductToRemoveIndex =
        basketToRemoveFrom.purchaseProducts.findIndex(
          (purchaseProduct) =>
            purchaseProduct.guid === action.payload.purchaseProductGuid
        );
      if (
        state.cart.baskets[basketToRemoveFromIndex].purchaseProducts.length ===
        1
      ) {
        return Object.assign({}, state, {
          cart: {
            baskets: [
              ...state.cart.baskets.slice(0, basketToRemoveFromIndex),
              ...state.cart.baskets.slice(basketToRemoveFromIndex + 1),
            ],
          },
        });
      }
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
            [basketToRemoveFromIndex]: newBasket,
          }),
        },
      });
    case UserActionTypes.CHANGE_PRODUCT_AMOUNT_IN_BASKET:
      const basketToChangeIndex = state.cart.baskets.findIndex(
        (basket) => basket.guid === action.payload.basketGuid
      );
      const basketToChange = state.cart.baskets[basketToChangeIndex];
      const purchaseProductToChangeIndex =
        basketToChange.purchaseProducts.findIndex(
          (purchaseProduct) =>
            purchaseProduct.guid === action.payload.purchaseProductGuid
        );
      const purchaseProductToChange =
        basketToChange.purchaseProducts[purchaseProductToChangeIndex];
      const { amount, ...changedPurhcaseProduct } = purchaseProductToChange;
      changedPurhcaseProduct["amount"] = parseInt(action.payload.newAmount, 10);
      const newPurchaseProducts = basketToChange.purchaseProducts.slice(0);
      newPurchaseProducts[purchaseProductToChangeIndex] =
        changedPurhcaseProduct;
      const newBasketAfterChange = {
        ...basketToChange,
        purchaseProducts: newPurchaseProducts,
      };
      return Object.assign({}, state, {
        cart: {
          baskets: Object.assign([], state.cart.baskets, {
            [basketToChangeIndex]: newBasketAfterChange,
          }),
        },
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
    case UserActionTypes.RECIEVE_ROLE_DISMISSAL_NOTIFICATION:
      return Object.assign({}, state, {
        notifications: [...state.notifications, action.payload.notification],
      });

    case UserActionTypes.RECEIVE_STORE_PURCHASE_NOTIFICATION:
      return Object.assign({}, state, {
        notifications: [...state.notifications, action.payload.notification],
      });

    case UserActionTypes.SEE_NOTIFICATION: {
      const seenNotificationIndex = action.payload.seenNotificationIndex;
      const seenNotification = action.payload.seenNotification;
      const newNotifcations = [
        ...state.notifications.slice(0, seenNotificationIndex),
        seenNotification,
        ...state.notifications.slice(seenNotificationIndex + 1),
      ];
      return Object.assign({}, state, {
        notifications: newNotifcations,
      });
    }
    case UserActionTypes.SUBMIT_PAYMENT_INFO: {
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
      });
    }
    case UserActionTypes.SUBMIT_DELIVERY_INFO: {
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
      });
    }
    case UserActionTypes.GET_CART_DISCOUNTED_PRICE_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
      });
    case UserActionTypes.GET_CART_DISCOUNTED_PRICE_SUCCESS: {
      console.log(action.payload.response);
      return Object.assign({}, state, {
        cart: Object.assign({}, state.cart, {
          discountedPrice: action.payload.response,
        }),
      });
    }
    case UserActionTypes.GET_CART_DISCOUNTED_PRICE_FAILURE:
      console.error(
        `error occured while getting cart discounted price: ${action.error}`
      );
      return state;
    case UserActionTypes.CREATE_PURCHASE_REQUEST:
      return Object.assign({}, state, {
        ...action.payload,
        error: undefined,
      });
    case UserActionTypes.CREATE_PURCHASE_SUCCESS: {
      return Object.assign({}, state, {
        paymentInfo: {},
        discountedPrice: {},
        isFetching: action.payload.isFetching,
      });
    }
    case UserActionTypes.CREATE_PURCHASE_FAILURE: {
      console.error(`error occured while purchasing cart: ${action.error}`);
      return Object.assign({}, state, {
        paymentInfo: {},
        discountedPrice: {},
        isFetching: action.payload.isFetching,
        error: action.error,
      });
    }

    default:
      return state;
  }
}
