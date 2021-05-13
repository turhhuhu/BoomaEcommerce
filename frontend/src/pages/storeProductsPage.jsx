import React, { Component } from "react";
import { connect } from "react-redux";
import ProductFilter from "../components/productFilter";
import Header from "../components/header";
import {
  fetchAllStoreProducts,
  filterStoreProducts,
} from "../actions/storeActions";
import StoreProductView from "../components/storeProductView";
import StoreSideBar from "../components/storeSideBar";
import StoreProductsHeader from "../components/storeProductsHeader";

class StoreProductsPage extends Component {
  state = {};

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };
  componentDidMount(prevProps) {
    if (this.props !== prevProps) {
      this.props.dispatch(fetchAllStoreProducts(this.props.match.params.guid));
    }
  }

  refreshStoreProducts = () => {
    this.props.dispatch(fetchAllStoreProducts(this.props.match.params.guid));
  };

  render() {
    return (
      <React.Fragment>
        <Header />
        <section className="section-content padding-y">
          <div className="container" style={{ maxWidth: "1500px" }}>
            <StoreProductsHeader
              storeName={this.props.storeInfo.storeName}
              guid={this.props.match.params.guid}
              refreshStoreProducts={this.refreshStoreProducts}
            />
            <br />
            <div className="row">
              <StoreSideBar
                isProducts="true"
                guid={this.props.guid}
                colClass="col-2"
              />
              <ProductFilter
                maxWidthStyle="290px"
                categories={[
                  ...new Set(
                    this.props.filteredProducts?.map(
                      (product) => product.category
                    )
                  ),
                ]}
                products={this.props.products}
                filteredProducts={this.props.filteredProducts}
                filterFunction={filterStoreProducts}
              />
              <main className="col-sm-6">
                <StoreProductView guid={this.props.match.params.guid} />
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
    products: store.store.products,
    filteredProducts: store.store.filteredProducts,
    storeInfo: store.store.storeInfo,
  };
};

export default connect(mapStateToProps)(StoreProductsPage);
