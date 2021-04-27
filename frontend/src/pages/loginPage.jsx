import React, { Component } from "react";
import LoginForm from "../components/loginForm";
import Header from "../components/header";
import { connect } from "react-redux";

class LoginPage extends Component {
  state = {};
  render() {
    return (
      <React.Fragment>
        <Header />
        <section className="section-conten padding-y">
          <LoginForm />
        </section>
      </React.Fragment>
    );
  }
}

export default connect()(LoginPage);
