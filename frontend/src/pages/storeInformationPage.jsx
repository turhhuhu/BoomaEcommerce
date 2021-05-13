import React, { Component } from "react";
import StoreInformation from "../components/storeInformation";
import { connect } from "react-redux";
import Header from "../components/header";
class StoreInformationPage extends Component {
  state = {};
  render() {
    return (
      <React.Fragment>
        <Header />
        <div className="container" style={{ maxWidth: "1000px" }}>
          <section className="section-conten padding-y">
            <StoreInformation guid={this.props.match.params.guid} />
          </section>
        </div>
      </React.Fragment>
    );
  }
}

export default connect()(StoreInformationPage);
