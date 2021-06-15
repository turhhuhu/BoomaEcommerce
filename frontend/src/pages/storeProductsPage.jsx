import React, { Component } from "react";
import { connect } from "react-redux";
import ProductFilter from "../components/productFilter";
import Header from "../components/header";
import {
  fetchAllStoreProducts,
  filterStoreProducts,
  fetchStoreInfo,
} from "../actions/storeActions";
import StoreProductView from "../components/storeProductView";
import StoreSideBar from "../components/storeSideBar";
import StoreProductsHeader from "../components/storeProductsHeader";
import { fetchUserStoreRole } from "../actions/userActions";

class StoreProductsPage extends Component {
  state = {};

  componentDidMount() {
    if (this.props.match.params.guid) {
      this.props.dispatch(fetchStoreInfo(this.props.match.params.guid));
      this.props.dispatch(fetchAllStoreProducts(this.props.match.params.guid));
      this.props.dispatch(fetchUserStoreRole(this.props.match.params.guid));
    }
  }

  render() {
    return (
      <React.Fragment>
        <Header />
        <section className="section-content padding-y">
          <div className="container" style={{ maxWidth: "1500px" }}>
            <StoreProductsHeader
              storeName={this.props.storeInfo.storeName}
              guid={this.props.match.params.guid}
            />
            <br />
            <div className="row">
              <StoreSideBar
                isProducts="true"
                guid={this.props.match.params.guid}
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
