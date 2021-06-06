import React, { Component } from "react";
import Header from "../components/header";
import DeliveryForm from "../components/deliveryForm";
class DeliveryPage extends Component {
  state = {};
  render() {
    return (
      <React.Fragment>
        <Header />
        <section className="section-conten padding-y">
          <DeliveryForm />
        </section>
      </React.Fragment>
    );
  }
}

export default DeliveryPage;
