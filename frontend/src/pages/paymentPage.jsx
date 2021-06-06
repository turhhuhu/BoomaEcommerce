import React, { Component } from "react";
import Header from "../components/header";
import { connect } from "react-redux";
import PaymentForm from "../components/paymentForm";

class PaymentPage extends Component {
  state = {};
  render() {
    return (
      <React.Fragment>
        <Header />
        <section className="section-conten padding-y">
          <PaymentForm />
        </section>
      </React.Fragment>
    );
  }
}

export default connect()(PaymentPage);
