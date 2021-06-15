import React from "react";
import ReactDOM from "react-dom";
import "bootstrap/dist/css/bootstrap.css";
import "font-awesome/css/font-awesome.min.css";
import { Provider } from "react-redux";
import { BrowserRouter, Route, Switch } from "react-router-dom";
import { Redirect } from "react-router";
import LoginPage from "./pages/loginPage";
import RegisterPage from "./pages/registerPage";
import ProductsPage from "./pages/productsPage";
import CartPage from "./pages/cartPage";
import { PersistGate } from "redux-persist/integration/react";
import configureStore from "./store";
import StoreInformationPage from "./pages/storeInformationPage";
import ProfilePage from "./pages/userProfilePage";
import UserStoresPage from "./pages/userStoresPage";
import StoreProductsPage from "./pages/storeProductsPage";
import StoreManagementPage from "./pages/storeManagementPage";
import NotificationsPage from "./pages/notificationsPage";
import StorePolicyPage from "./pages/storePolicyPage";
import PaymentPage from "./pages/paymentPage";
import DeliveryPage from "./pages/deliveryPage";
import CartReviewPage from "./pages/cartReviewPage";
import PurchaseReviewPage from "./pages/purchaseReviewPage";
import StoreDiscountsPage from "./pages/storeDiscountsPage";
import StoreDiscountPolicyPage from "./pages/StoreDiscountPolicyPage";
import GuestInformationPage from "./pages/guestInformationPage";
import UserPurchaseHistoryPage from "./pages/userPurchaseHistoryPage";
import StorePurchaseHistoryPage from "./pages/storePurchaseHistoryPage";
import UserPurchaseHistoryDetailsPage from "./pages/userPurchaseHistoryDetailsPage";
import storePurchaseHistoryDetailsPage from "./pages/storePurchaseHistoryDetailsPage";

const { store, persistor } = configureStore();

ReactDOM.render(
  <Provider store={store}>
    <PersistGate loading={null} persistor={persistor}>
      <BrowserRouter>
        <Switch>
          <Route path="/register" component={RegisterPage}></Route>
          <Route path="/login" component={LoginPage}></Route>
          <Route path="/products" component={ProductsPage}></Route>
          <Route path="/home" component={ProductsPage}></Route>
          <Route path="/cart/guest" component={GuestInformationPage}></Route>
          <Route path="/cart/review" component={CartReviewPage}></Route>
          <Route path="/cart/purchase" component={PurchaseReviewPage}></Route>
          <Route path="/cart/payment" component={PaymentPage}></Route>
          <Route path="/cart/delivery" component={DeliveryPage}></Route>
          <Route path="/cart" component={CartPage}></Route>
          <Route exact path="/user/stores" component={UserStoresPage}></Route>
          <Route
            exact
            path="/user/purchases/:guid"
            component={UserPurchaseHistoryDetailsPage}
          ></Route>
          <Route
            exact
            path="/user/purchases"
            component={UserPurchaseHistoryPage}
          ></Route>
          <Route
            exact
            path="/user/notifications"
            component={NotificationsPage}
          ></Route>
          <Route exact path="/user" component={ProfilePage}></Route>
          <Route
            exact
            path="/store/:guid/discounts/:discountGuid"
            component={StoreDiscountPolicyPage}
          ></Route>
          <Route
            exact
            path="/store/:guid/purchases/:guid"
            component={storePurchaseHistoryDetailsPage}
          ></Route>
          <Route
            exact
            path="/store/:guid/purchases"
            component={StorePurchaseHistoryPage}
          ></Route>
          <Route
            exact
            path="/store/:guid/discounts"
            component={StoreDiscountsPage}
          ></Route>
          <Route
            exact
            path="/store/:guid/products"
            component={StoreProductsPage}
          ></Route>
          <Route
            exact
            path="/store/:guid/management"
            component={StoreManagementPage}
          ></Route>
          <Route
            exact
            path="/store/:guid/policy"
            component={StorePolicyPage}
          ></Route>
          <Route
            exact
            path="/store/:guid"
            component={StoreInformationPage}
          ></Route>
          <Route path="/">
            <Redirect to="/home" />;
          </Route>
        </Switch>
      </BrowserRouter>
    </PersistGate>
  </Provider>,
  document.getElementById("root")
);
