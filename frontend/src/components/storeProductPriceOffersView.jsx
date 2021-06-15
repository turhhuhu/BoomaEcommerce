import React, { Component } from "react";
import { connect } from "react-redux";
import StoreProductPriceOfferEntry from "./storeProductPriceOfferEntry";

class StoreProductPriceOffersView extends Component {
  state = {};
  render() {
    return (
      <div className="col">
        <article className="card mb-3">
          <header
            className="card-header pl-3"
            style={{ backgroundColor: "rgba(69, 136, 236, 0.125)" }}
          >
            <h5>Product Offers</h5>
          </header>
          <div
            className="table-responsive"
            style={{
              display: "block",
              maxHeight: "700px",
              overflowY: "scroll",
            }}
          >
            <table className="table table-hover">
              <tbody>
                {this.props.productOffers.map((productOffer, index) => (
                  <StoreProductPriceOfferEntry
                    key={index}
                    productOffer={productOffer}
                  />
                ))}
              </tbody>
            </table>
          </div>
        </article>
      </div>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    productOffers: store.store.productOffers,
  };
};

export default connect(mapStateToProps)(StoreProductPriceOffersView);
