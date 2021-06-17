import React, { Component } from "react";
import GuestInformationForm from "../components/guestInformationForm";
import Header from "../components/header";
class GuestInformationPage extends Component {
  state = {};
  render() {
    return (
      <React.Fragment>
        <Header />
        <section className="section-conten padding-y">
          <GuestInformationForm />
        </section>
      </React.Fragment>
    );
  }
}

export default GuestInformationPage;
