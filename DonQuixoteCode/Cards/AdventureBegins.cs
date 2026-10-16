using DonQuixote.Chains;
using DonQuixote.Characters;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DonQuixote.Cards;

[RegisterCard(typeof(DonQuixoteCardPool))]
[RegisterCharacterStarterCard(typeof(DonQuixoteCharacter), 1)]
public sealed class AdventureBegins : ChainLinkBase
{
    protected override int ChainIndex => 1;
}