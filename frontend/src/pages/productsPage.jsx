import React, { Component } from "react";
import { connect } from "react-redux";
import ProductView from "../components/productView";
import ProductFilter from "../components/productFilter";
import Header from "../components/header";
import "../css/productsPage.css";

class ProductPage extends Component {
  state = {};
  render() {
    return (
      <React.Fragment>
        <Header />
        <section className="section-content padding-y">
          <div className="container">
            <div className="row">
              <ProductFilter />
              <main className="col-md-9">
                <ProductView />
              </main>
            </div>
          </div>
        </section>
      </React.Fragment>
    );
  }
}

export default connect()(ProductPage);
