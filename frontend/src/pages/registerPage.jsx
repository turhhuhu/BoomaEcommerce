import React, { Component } from "react";
import SignUpForm from "../components/signupForm";
import Header from "../components/header";
import { connect } from "react-redux";

class RegisterPage extends Component {
  state = {};
  render() {
    return (
      <React.Fragment>
        <Header />
        <main className="container">
          <section className="section-conten padding-y">
            <SignUpForm />
          </section>
        </main>
      </React.Fragment>
    );
  }
}

export default connect()(RegisterPage);
