import React, { Component } from "react";
import { connect } from "react-redux";
import UserProductPriceOfferEntry from "./userProductPriceOfferEntry";

class UserProductPriceOfferView extends Component {
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
            {this.props.isFetching ? (
              <div className="d-flex justify-content-center">
                <div
                  className="spinner-border text-primary"
                  role="status"
                ></div>
              </div>
            ) : null}
            <table className="table table-hover">
              <tbody>
                {this.props.productOffers?.map((productOffer, index) => (
                  <UserProductPriceOfferEntry
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
    productOffers: store.user.productOffers,
    isFetching: store.user.isFetching,
  };
};

export default connect(mapStateToProps)(UserProductPriceOfferView);
