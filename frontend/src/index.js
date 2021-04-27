import React from "react";
import ReactDOM from "react-dom";
import "bootstrap/dist/css/bootstrap.css";
import "font-awesome/css/font-awesome.min.css";
import { Provider } from "react-redux";
import { createStore, applyMiddleware } from "redux";
import { BrowserRouter, Route, Switch } from "react-router-dom";
import thunkMiddleware from "redux-thunk";
import api from "./middleware/api";
import combinedReducers from "./reducers/combinedReducers";
import LoginPage from "./pages/loginPage";
import RegisterPage from "./pages/registerPage";
import ProductsPage from "./pages/productsPage";

let createStoreWithMiddleware = applyMiddleware(
  thunkMiddleware,
  api
)(createStore);
let store = createStoreWithMiddleware(
  combinedReducers,
  window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__()
);

ReactDOM.render(
  <Provider store={store}>
    <BrowserRouter>
      <Switch>
        <Route path="/register">
          <RegisterPage />
        </Route>
        <Route path="/login">
          <LoginPage />
        </Route>
        <Route path="/products">
          <ProductsPage />
        </Route>
        <Route path="/home">
          <ProductsPage />
        </Route>
        <Route path="/">
          <ProductsPage />
        </Route>
      </Switch>
    </BrowserRouter>
  </Provider>,
  document.getElementById("root")
);
