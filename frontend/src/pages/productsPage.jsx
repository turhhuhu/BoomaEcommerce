import React, { Component } from "react";
import { connect } from "react-redux";
import ProductView from "../components/productView";
import ProductFilter from "../components/productFilter";
import Header from "../components/header";
import "../css/productsPage.css";
import { fetchAllProducts } from "../actions/productsActions";
class ProductPage extends Component {
  state = {};

  componentDidMount(prevProps) {
    if (this.props !== prevProps) {
      this.props.dispatch(fetchAllProducts());
    }
  }

  render() {
    return (
      <React.Fragment>
        <Header />
        <section className="section-content padding-y">
          <div className="container">
            <div className="row">
              <ProductFilter
                categories={[
                  ...new Set(
                    this.props.products?.map((product) => product.category)
                  ),
                ]}
              />
              <main className="col-md-9">
                <ProductView products={this.props.products} />
              </main>
            </div>
          </div>
        </section>
      </React.Fragment>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    products: store.productsView.products,
    authenticated: store.auth.authenticated,
  };
};

export default connect(mapStateToProps)(ProductPage);
