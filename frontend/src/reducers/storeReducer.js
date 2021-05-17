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
      });
    case StoreActionTypes.ADD_PRODUCT_TO_STORE_SUCCESS: {
      const newProducts = [...state.products, action.payload.response];
      return Object.assign({}, state, {
        products: newProducts,
        filteredProducts: newProducts,
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
      });
    case StoreActionTypes.EDIT_PRODUCT_FROM_STORE_FAILURE:
      console.error(
        `error occured while editing store product: ${action.error}`
      );
      return Object.assign({}, state, {
        error: action.error,
        isFetching: action.payload.isFetching,
      });
    default:
      return state;
  }
}
