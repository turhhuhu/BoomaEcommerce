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
import StorePage from "./pages/storePage";

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
          <Route path="/cart" component={CartPage}></Route>
          <Route path="/store/:guid" component={StorePage}></Route>
          <Route path="/">
            <Redirect to="/home" />;
          </Route>
        </Switch>
      </BrowserRouter>
    </PersistGate>
  </Provider>,
  document.getElementById("root")
);
