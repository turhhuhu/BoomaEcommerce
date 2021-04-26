import React from "react";
import ReactDOM from "react-dom";
import "bootstrap/dist/css/bootstrap.css";
import { Provider } from "react-redux";
import { createStore, applyMiddleware } from "redux";
import { BrowserRouter, Route, Switch } from "react-router-dom";
import thunkMiddleware from "redux-thunk";
import api from "./middleware/api";
import LoginForm from "./components/login";
import SignUpForm from "./components/signup";
import combinedReducers from "./reducers/combinedReducers";

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
        <Route path="/signup">
          <SignUpForm />
        </Route>
        <Route path="/login">
          <LoginForm />
        </Route>
        <Route path="/">
          <LoginForm />
        </Route>
      </Switch>
    </BrowserRouter>
  </Provider>,
  document.getElementById("root")
);
